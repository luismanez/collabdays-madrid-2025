using System.Text.Json;
using System.Text.Json.Serialization;
using Abyx.SharePoint.Lists.Ingestor.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.KernelMemory;

namespace Abyx.SharePoint.Lists.Ingestor.HostedServices;

public class ProductsIngestorHostedService(
    GraphProductService graphProductService,
    MemoryServerless memoryServerless) : IHostedService
{
    private readonly GraphProductService _graphProductService = graphProductService;
    private readonly MemoryServerless _memoryServerless = memoryServerless;

    public static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var products = await _graphProductService.GetProductsAsync(cancellationToken);

        var jsonOptions = GetJsonOptions();
        foreach (var product in products)
        {
            Console.WriteLine($"Ingesting product: {product.SKU}");
            var json = JsonSerializer.Serialize(product, jsonOptions);
            await _memoryServerless.ImportTextAsync(json, product.SKU, cancellationToken: cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("ExpertsIndexerHostedService is stopping.");
        return Task.CompletedTask;
    }
}
