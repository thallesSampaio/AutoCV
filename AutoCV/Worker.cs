using AutoCV.Services;

namespace AutoCV
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DataScraperService _scraper;
        private readonly ZipService _zipService;
        private readonly CsvProcessor _csvProcessor;
        public Worker(ILogger<Worker> logger, DataScraperService scraper, ZipService zipService, CsvProcessor csvProcessor)
        {
            _logger = logger;
            _scraper = scraper;
            _zipService = zipService;
            _csvProcessor = csvProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker rodando às: {time}", DateTimeOffset.Now);

                        //await _scraper.DownloadFilesAsync(stoppingToken);
                        //await _zipService.ProcessZipFilesAsync(stoppingToken);
                        _csvProcessor.ProcessCsv();
                }

                await Task.Delay(2000, stoppingToken);
            }
        }

        //private async Task RunPipelineAsync(CancellationToken token)
        //{
        //    await _scraper.DownloadFilesAsync(token);
        //    await _zipService.ProcessZipFilesAsync(token);
        //}
    }
}