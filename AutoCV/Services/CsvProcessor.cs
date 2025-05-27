using System.Text;
using AutoCV.Entities;
using AutoCV.Utils;
using CsvHelper;
using CsvHelper.Configuration;
using static System.Console;

namespace AutoCV.Services
{
    public class CsvProcessor
    {
        private readonly string _sourceDirectory;
        private readonly ILogger<CsvProcessor> _logger;

        public CsvProcessor(IConfiguration configuration, ILogger<CsvProcessor> logger)
        {
            _sourceDirectory = configuration["Scraper:DownloadPath"]
                ?? throw new ArgumentNullException("DownloadPath not found.");
            _logger = logger;
        }

        public void ProcessCsv()
        {
            string[] directories = Directory.GetDirectories(_sourceDirectory);
            foreach (string directory in directories)
            {
                string[] csvFile = Directory.GetFiles(directory);

                foreach (var csvv in csvFile)
                {
                    //ProcessFile<Cnae, CnaeMap>(csvv);
                    ProcessFile<Empresa, EmpresaMap>(csvv);
                    Console.WriteLine(csvv);
                    Console.WriteLine($"{directory}");
                }


            }
        }

        private void ProcessFile<TEntity, TMap>(string filePath) where TEntity : class where TMap : ClassMap<TEntity>
        {
            try
            {
                var entities = ParseToEntities<TEntity, TMap>(filePath);

                foreach (var entity in entities)
                {
                    if (entity is Cnae cnae)
                    {
                        Console.WriteLine($"Código: {cnae.Codigo} | Descrição: {cnae.Descricao}");
                    }
                    else if (entity is Empresa empresa)
                    {
                        Console.WriteLine($"CNPJ: {empresa.Cnpj} | Nome: {empresa.Nome} | Email: {empresa.Email} UF: {empresa.Uf} " +
                            $"CNAE 1: {empresa.CnaePrincipal} cnae2: {empresa.CnaesSecundarios}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar {filePath}: {ex.Message}");
            }

            // await _repository.InsertDataAsync(entities);
        }

        //public List<T> ParseToEntities<T, TMap>(string filePathCsv) where TMap : ClassMap<T> where T : class
        //{
        //    using var reader = new StreamReader(filePathCsv, Encoding.UTF8);
        //    using var csv = new CsvReader(reader, CsvConfig.config);

        //    csv.Context.RegisterClassMap<TMap>();
        //    var csvDataList = csv.GetRecords<T>().ToList();
        //    return csvDataList;
        //}

        public List<T> ParseToEntities<T, TMap>(string filePathCsv)
    where TMap : ClassMap<T>
    where T : class
        {
            var list = new List<T>();

            using var reader = new StreamReader(filePathCsv, Encoding.UTF8);
            using var csv = new CsvReader(reader, CsvConfig.config);

            csv.Context.RegisterClassMap<TMap>();

            foreach (var record in csv.GetRecords<T>().Take(1000000))
            {
                list.Add(record);
            }

            return list;
        }
    }
} 