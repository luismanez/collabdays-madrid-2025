using Abyx.SharePoint.Lists.Ingestor.Extensions;
using Abyx.SharePoint.Lists.Ingestor.HostedServices;
using Abyx.SharePoint.Lists.Ingestor.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        configHost.SetBasePath(currentDirectory);
        configHost.AddJsonFile("appsettings.json", optional: false);
        configHost.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddApplicationOptions(configuration);
        services.AddKernelMemory(configuration);
        services.AddSingleton<GraphProductService>();

        services.AddLogging(configure => configure.AddConsole());

        services.AddHostedService<ProductsIngestorHostedService>();
    })
    .Build();

host.Run();
