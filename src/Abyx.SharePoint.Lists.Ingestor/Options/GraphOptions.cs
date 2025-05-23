namespace Abyx.SharePoint.Lists.Ingestor.Options;

public class GraphOptions
{
    public static string SettingsSectionName => "GraphSettings";
    public string TenantId { get; init; } = default!;
    public string ClientId { get; init; } = default!;
    public string ClientSecret { get; init; } = default!;
    public string SiteId { get; init; } = default!;
    public string ListId { get; init; } = default!;
}
