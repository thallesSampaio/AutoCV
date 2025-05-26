using AutoCV.Services;

namespace AutoCV
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DataScraperService _scraper;
        private readonly ZipService _extractZip;
        public Worker(ILogger<Worker> logger, DataScraperService scraper, ZipService extractZip)
        {
            _logger = logger;
            _scraper = scraper;
            _extractZip = extractZip;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker rodando às: {time}", DateTimeOffset.Now);

                        await _scraper.DownloadFilesAsync(stoppingToken);
                        await _extractZip.ExtractAndDeleteFiles();
                }

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}