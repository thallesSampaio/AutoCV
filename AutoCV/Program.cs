using System.Net;

class Program 
{
    public static void Main()
    {
        string remoteUri = "https://arquivos.receitafederal.gov.br/dados/cnpj/dados_abertos_cnpj/2025-05/";
        string fileName = "Cnaes.zip";
        string dest = @$"G:\\dev\\{fileName}";

        using (WebClient webClient = new WebClient())
        {
            Console.WriteLine("Downloading file {0}...", fileName);

            // Descarga el recurso web y guárdalo en el destino
            webClient.DownloadFile(remoteUri + fileName, dest);

            Console.WriteLine("Successfully downloaded file {0}", fileName);
        }
    }
}