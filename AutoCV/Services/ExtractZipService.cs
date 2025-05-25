using System.IO;
using System.IO.Compression;

namespace AutoCV.Services
{
    public class ExtractZipService
    {
        private readonly string _sourceDirectory;

        public ExtractZipService(IConfiguration configuration)
        {
            _sourceDirectory = configuration["Scraper:DownloadPath"]
                ?? throw new ArgumentNullException("DownloadPath not found.");
        }

        
        public async Task ExtractFiles()
        {
            string[] zipFiles = Directory.GetFiles(_sourceDirectory, "*.zip");
            
            foreach (string zipPath in zipFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(zipPath);

                //to do - logs
                Console.WriteLine($"Extracting: {fileName}...");

                ZipFile.ExtractToDirectory(zipPath, _sourceDirectory, overwriteFiles: true);

                Console.WriteLine($"File extracted in: {_sourceDirectory}");
            }
        }

        // to do - remove folder after extract files
        // to do - maybe insert the data after extracting and delete the folder,
        // because the CSV file has a strange name with weird formatting,
        // and that will give me unnecessary work later
    }
}
