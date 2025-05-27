using AutoCV.Services;

namespace AutoCV
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DataScraperService _scraper;
        private readonly ZipService _zipService;
        public Worker(ILogger<Worker> logger, DataScraperService scraper, ZipService zipService)
        {
            _logger = logger;
            _scraper = scraper;
            _zipService = zipService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker rodando �s: {time}", DateTimeOffset.Now);

                        await _scraper.DownloadFilesAsync(stoppingToken);
                        await _zipService.ProcessZipFilesAsync(stoppingToken);
                }

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}