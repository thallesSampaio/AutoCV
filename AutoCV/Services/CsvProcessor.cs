using System.Diagnostics;
using System.Text;
using AutoCV.Entities;
using AutoCV.Repositories;
using AutoCV.Utils;
using CsvHelper;
using CsvHelper.Configuration;

namespace AutoCV.Services
{
    public class CsvProcessor
    {
        private readonly string _sourceDirectory;
        private readonly ILogger<CsvProcessor> _logger;
        private readonly GenericRepository _repository;

        public CsvProcessor(IConfiguration configuration, GenericRepository genericRepository, ILogger<CsvProcessor> logger)
        {
            _sourceDirectory = configuration["Scraper:DownloadPath"]
                ?? throw new ArgumentNullException("DownloadPath not found.");
            _logger = logger;
            _repository = genericRepository;
        }

        public void ProcessCsv()
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount / 2 };
            int totalEmpresas = 0;
            int totalCnaes = 0;
            var stopwatchTotal = Stopwatch.StartNew();

            _logger.LogInformation("Starting processing CSV files in directory: {Directory}", _sourceDirectory);

            string[] directories = Directory.GetDirectories(_sourceDirectory);
            foreach (string directory in directories)
            {
                string[] csvFiles = Directory.GetFiles(directory);

                foreach (var csv in csvFiles)
                {
                    try
                    {
                        var stopwatchFile = Stopwatch.StartNew();
                        string type = Path.GetExtension(csv);
                        int processedCount = 0;

                        if (type == ".ESTABELE")
                        {
                            processedCount = ProcessFileStreaming<Empresa, EmpresaMap>(csv, "empresas_teste", 10000);
                            totalEmpresas += processedCount;
                        }
                        else if (type == ".CNAECSV")
                        {
                            processedCount = ProcessFileStreaming<Cnae, CnaeMap>(csv, "cnae_teste", 10000);
                            totalCnaes += processedCount;
                        }

                        stopwatchFile.Stop();

                        _logger.LogInformation("""
                            File processed: {FileName}
                            Directory: {Directory}
                            Records processed: {TotalProcessed}
                            Processing time: {ElapsedTime}s
                            Type: {FileType}
                            """,
                            Path.GetFileName(csv),
                            directory,
                            processedCount,
                            stopwatchFile.Elapsed.TotalSeconds,
                            type);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing file: {FileName}", csv);
                    }
                }
            }

            stopwatchTotal.Stop();

            _logger.LogInformation("""
                FINAL RESUME:
                Total number of empresas processed: {TotalEmpresas}
                Total number of CNAEs processed: {TotalCnaes}
                Total execution time: {TotalTime}s
                Average: {AverageRecords} records/second
                """,
                totalEmpresas,
                totalCnaes,
                stopwatchTotal.Elapsed.TotalSeconds,
                (totalEmpresas + totalCnaes) / stopwatchTotal.Elapsed.TotalSeconds);
        }

        private int ProcessFileStreaming<T, TMap>(string filePath, string tableName, int batchSize)
            where T : class where TMap : ClassMap<T>
        {
            int totalProcessed = 0;
            var batch = new List<T>(batchSize);

            using var reader = new StreamReader(filePath, Encoding.UTF8);
            using var csv = new CsvReader(reader, CsvConfig.config);
            csv.Context.RegisterClassMap<TMap>();

            foreach (var record in csv.GetRecords<T>())
            {
                batch.Add(record);
                if (batch.Count >= batchSize)
                {
                    try
                    {
                        _repository.BulkInsert(tableName, batch);
                        totalProcessed += batch.Count;

                        _logger.LogDebug("Inserted batch of {BatchSize} records into table {TableName}. Total: {TotalProcessed}",
                            batch.Count, tableName, totalProcessed);

                        batch.Clear();

                        if (totalProcessed % 100_000 == 0)
                        {
                            _logger.LogInformation("Progress marker: {TotalProcessed} records processed", totalProcessed);
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error inserting batch into table {TableName}", tableName);
                        throw;
                    }
                }
            }

            // Processar último lote
            if (batch.Count > 0)
            {
                _repository.BulkInsert(tableName, batch);
                totalProcessed += batch.Count;
                _logger.LogDebug("Last batch with {BatchSize} records inserted", batch.Count);
            }

            return totalProcessed;
        }
    }
}