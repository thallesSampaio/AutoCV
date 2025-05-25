using System.Diagnostics;
using System.Net;
using AutoCV.Services;
using HtmlAgilityPack;

namespace AutoCV
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DataScraperService _scraper;
        public Worker(ILogger<Worker> logger, DataScraperService scraper)
        {
            _logger = logger;
            _scraper = scraper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker rodando às: {time}", DateTimeOffset.Now);

                        await _scraper.DownloadFilesAsync(stoppingToken);
                }

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}