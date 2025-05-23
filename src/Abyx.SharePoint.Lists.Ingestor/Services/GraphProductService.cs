using Abyx.SharePoint.Lists.Ingestor.Models;
using Abyx.SharePoint.Lists.Ingestor.Options;
using Azure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace Abyx.SharePoint.Lists.Ingestor.Services;

public class GraphProductService
{
    private const string GraphDefaultScope = "https://graph.microsoft.com/.default";
    private readonly GraphOptions _settings;
    private readonly GraphServiceClient _graph;

    public GraphProductService(IOptions<GraphOptions> options)
    {
        _settings = options.Value;

        var credential = new ClientSecretCredential(
            _settings.TenantId,
            _settings.ClientId,
            _settings.ClientSecret);

        _graph = new GraphServiceClient(
            credential,
            [GraphDefaultScope]);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(
        CancellationToken cancel = default)
    {
        var page = await _graph
            .Sites[_settings.SiteId]
            .Lists[_settings.ListId]
            .Items
            .GetAsync(cfg =>
            {
                cfg.QueryParameters.Expand = ["fields"];
            }, cancel);

        if (page?.Value == null) return [];

        return [.. page.Value.Select(it =>
            new Product(
                it.Fields?.AdditionalData?[Constants.Fields.SKU]?.ToString(),
                it.Fields?.AdditionalData?[Constants.Fields.ProductName]?.ToString(),
                it.Fields?.AdditionalData?[Constants.Fields.Description]?.ToString(),
                it.Fields?.AdditionalData?[Constants.Fields.Category]?.ToString(),
                it.Fields?.AdditionalData?[Constants.Fields.Stock] is int stockValue ? (int?)stockValue :
                int.TryParse(it.Fields?.AdditionalData?[Constants.Fields.Stock]?.ToString(), out var parsedStock) ? parsedStock : null,
                DateTime.TryParse(it.Fields?.AdditionalData?[Constants.Fields.EntryDate]?.ToString(), out var parsedEntryDate) ? parsedEntryDate : null,
                int.TryParse(it.Fields?.AdditionalData?[Constants.Fields.Price]?.ToString(), out var parsedPrice) ? parsedPrice : null
                ))
            .Where(p => p.SKU is not null && p.ProductName is not null)];
    }
}
