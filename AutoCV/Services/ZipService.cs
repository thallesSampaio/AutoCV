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


        public Task ProcessZipFilesAsync(CancellationToken cancellationToken)
        {
            string[] zipFiles = Directory.GetFiles(_sourceDirectory, "*.zip");

            if (zipFiles.Length == 0)
            {
                _logger.LogInformation("No zip files found to process.");
                return Task.FromCanceled(cancellationToken);
            }
            foreach (string zipPath in zipFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(zipPath);

                _logger.LogInformation($"Extracting: {fileName}...");
                ZipFile.ExtractToDirectory(zipPath, Path.Combine(_sourceDirectory, fileName), overwriteFiles: true);
                _logger.LogInformation($"File extracted in: {_sourceDirectory}");
                File.Delete(zipPath);
                _logger.LogInformation($"Zip file deleted: {zipPath}");
            }
            return Task.CompletedTask;
        }

        //to do - in future, after a csv get extracted start to insert when exctracting the other, no wait everything gets extracted to start insert

    }
}
