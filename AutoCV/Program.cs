using AutoCV;
using AutoCV.Data;
using AutoCV.Repositories;
using AutoCV.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<GenericRepository>();   
        services.AddSingleton<CsvProcessor>();          
        services.AddSingleton<DapperContext>();      
        services.AddSingleton<DataScraperService>();
        services.AddSingleton<ZipService>();
        services.AddSingleton(new HttpClient());
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();