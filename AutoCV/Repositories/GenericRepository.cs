using System.Data;
using System.Data.SqlClient;
using AutoCV.Data;
using AutoCV.Entities;

namespace AutoCV.Repositories
{
    public class GenericRepository
    {
        private readonly DapperContext _dapperContext;

        public GenericRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public void BulkInsert<TEntity>(string tableName, IEnumerable<TEntity> entities)
        {
            if (entities == null || !entities.Any())
            {
                Console.WriteLine("No data to insert.");
                return;
            }

            DataTable table = ToDataTable(entities);

            using (var connection = _dapperContext.CreateConnection())
            {
                var sqlConnection = connection as SqlConnection;
                if (sqlConnection == null)
                {
                    throw new InvalidOperationException("The connection must be of type SqlConnection to use SqlBulkCopy");
                }

                using (var bulkCopy = new SqlBulkCopy(sqlConnection))
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.EnableStreaming = true;
                    bulkCopy.BatchSize = 10000;
                    bulkCopy.NotifyAfter = 5000;

                    if (sqlConnection.State != ConnectionState.Open)
                    {
                        sqlConnection.Open();
                    }

                    try
                    {
                        bulkCopy.WriteToServer(table);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Bulk insert error: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        private DataTable ToDataTable<TEntity>(IEnumerable<TEntity> entities)
        {
            var dataTable = new DataTable();
            var props = typeof(TEntity).GetProperties();

            if(typeof(TEntity) == typeof(Empresa)) 
            {
                IEnumerable<Empresa> empresas = (IEnumerable<Empresa>)entities;
                dataTable.Columns.Add("cnpj", typeof(string));         
                dataTable.Columns.Add("nome", typeof(string));          
                dataTable.Columns.Add("cnae_principal", typeof(string));
                dataTable.Columns.Add("cnae_secundarios", typeof(string));
                dataTable.Columns.Add("uf", typeof(string));            
                dataTable.Columns.Add("municipio", typeof(string));    
                dataTable.Columns.Add("telefone", typeof(string));      
                dataTable.Columns.Add("email", typeof(string));

                foreach (var empresa in empresas)
                {
                    dataTable.Rows.Add(
                        empresa.Cnpj,    
                        empresa.Nome,
                        empresa.CnaePrincipal,
                        empresa.CnaesSecundarios,
                        empresa.Uf,
                        empresa.Municipio,
                        empresa.Telefone,
                        empresa.Email
                    );
                }
                return dataTable;
            }

            foreach (var prop in props)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataTable.Columns.Add(prop.Name, type);
            }

            foreach (var entity in entities)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(entity) ?? DBNull.Value;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}