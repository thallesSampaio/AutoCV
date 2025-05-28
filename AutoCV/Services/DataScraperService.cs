using System.Diagnostics;
using HtmlAgilityPack;

namespace AutoCV.Services
{
    public class DataScraperService
    {
        private readonly ILogger<DataScraperService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _downloadPath;
        private readonly string _url;

        public DataScraperService(
            ILogger<DataScraperService> logger,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _downloadPath = configuration["Scraper:DownloadPath"] ?? throw new ArgumentNullException("DownloadPath not configured");
            _url = configuration["Scraper:BaseUrl"] ?? throw new ArgumentNullException("BaseUrl not configured");
        }

        private bool NoFilesExist()
        {
            if (!Directory.Exists(_downloadPath))
            {
                Directory.CreateDirectory(_downloadPath);
                return true;
            }
            var files = Directory.GetFiles(_downloadPath, "*.zip");
            return files.Length == 0;
        }

        public async Task DownloadFilesAsync(CancellationToken cancellationToken)
        {
            if (NoFilesExist()) 
            {
                var links = ExtractDownloadLinks();
                if (links.Count == 0)
                {
                    _logger.LogWarning("No relevant zip found.");
                    return;
                }

                foreach (var link in links)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    string remoteUri = _url + link;
                    string destinationPath = Path.Combine(_downloadPath, link);

                    try
                    {
                        await DownloadFileAsync(remoteUri, destinationPath, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogWarning("Download canceled for: {File}", link);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while downloading file: {File}", link);
                    }
                }
            }
            _logger.LogInformation("Data already exists.");
        }

        private List<string> ExtractDownloadLinks()
        {
            var htmlWeb = new HtmlWeb();
            var htmlDoc = htmlWeb.Load(_url);
            var nodes = htmlDoc.DocumentNode.SelectNodes("//a");

            if (nodes == null)
                return new List<string>();

            return nodes
                .Select(node => node.GetAttributeValue("href", ""))
                .Where(href =>
                    (href.Contains("Estabelecimentos") ||
                     href.Contains("Cnaes") ||
                     href.Contains("Municipios")) &&
                    href.EndsWith(".zip"))
                .ToList();
        }

        private async Task DownloadFileAsync(string url, string destinationPath, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting download: {File}", Path.GetFileName(destinationPath));
            var stopwatch = Stopwatch.StartNew();

            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var fileStream = File.Create(destinationPath);

            var buffer = new byte[81920];
            long totalBytesRead = 0;
            int bytesRead;
            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            long previousBytesRead = 0;
            var lastUpdateTime = stopwatch.Elapsed;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                totalBytesRead += bytesRead;

                if (totalBytes > 0)
                {
                    var percent = (int)((totalBytesRead * 100L) / totalBytes);
                    float readMB = totalBytesRead / 1024f / 1024f;
                    float totalMB = totalBytes / 1024f / 1024f;

                    var currentTime = stopwatch.Elapsed;
                    var elapsedSeconds = (currentTime - lastUpdateTime).TotalSeconds;

                    double speedMBps = 0;

                    if (elapsedSeconds > 0.5)
                    {
                        speedMBps = (totalBytesRead - previousBytesRead) / 1024f / 1024f / elapsedSeconds;
                        previousBytesRead = totalBytesRead;
                        lastUpdateTime = currentTime;

                        Console.Write(
                            $"\rFile: {Path.GetFileName(destinationPath),-20} " +
                            $"Progress: {percent,3}% " +
                            $"({readMB:F1}/{totalMB:F1} MB) " +
                            $"Speed: {speedMBps:F2} MB/s");
                    }
                }
            }

            stopwatch.Stop();
            _logger.LogInformation("Download completed: {File} em {Seconds:F1}s", Path.GetFileName(destinationPath), stopwatch.Elapsed.TotalSeconds);
        }
    }
}