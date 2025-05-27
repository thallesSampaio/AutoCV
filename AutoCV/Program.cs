using AutoCV;
using AutoCV.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton(new HttpClient());
        services.AddSingleton<DataScraperService>();
        services.AddSingleton<ZipService>();
        services.AddSingleton<CsvProcessor>();
        services.AddHostedService<Worker>(); 
    })
    .Build();

await host.RunAsync();