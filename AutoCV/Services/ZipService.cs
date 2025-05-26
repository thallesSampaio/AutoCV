using System.IO.Compression;

namespace AutoCV.Services
{
    public class ZipService
    {
        private readonly string _sourceDirectory;
        private readonly ILogger<ZipService> _logger;

        public ZipService(IConfiguration configuration, ILogger<ZipService> logger)
        {
            _sourceDirectory = configuration["Scraper:DownloadPath"]
                ?? throw new ArgumentNullException("DownloadPath not found.");
            _logger = logger;
        }


        public async Task ExtractAndDeleteFiles()
        {
            string[] zipFiles = Directory.GetFiles(_sourceDirectory, "*.zip");
            
            foreach (string zipPath in zipFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(zipPath);

                //to do - logs
                _logger.LogInformation($"Extracting: {fileName}...");

                ZipFile.ExtractToDirectory(zipPath, Path.Combine(_sourceDirectory, fileName), overwriteFiles: true);
                _logger.LogInformation($"File extracted in: {_sourceDirectory}");
                File.Delete(zipPath);
                _logger.LogInformation($"Zip file deleted: {zipPath}");
            }
        }

        //string[] directories = Directory.GetDirectories(_sourceDirectory);
        //        foreach (string directory in directories)
        //        {
        //            Console.WriteLine($"{directory}");
        //        }

    // to do - remove folder after extract files
    // to do - maybe insert the data after extracting and delete the folder,
    // because the CSV file has a strange name with weird formatting,
    // and that will give me unnecessary work later
}
}
