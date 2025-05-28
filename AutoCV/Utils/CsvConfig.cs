using System.Globalization;
using System.Text;
using CsvHelper.Configuration;

namespace AutoCV.Utils
{
    public class CsvConfig
    {
        public readonly static CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false, // Caso haja cabeçalho
            Delimiter = ";", // Ajuste se o delimitador for ponto e vírgula
            BadDataFound = null, // Ignorar dados inválidos, caso necessário
            MissingFieldFound = null,
            IgnoreBlankLines = true,
            HeaderValidated = null,
        };
    }
}