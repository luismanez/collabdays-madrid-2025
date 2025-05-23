namespace Abyx.SharePoint.Lists.Ingestor.Models;

public record Product(
    string? SKU,
    string? ProductName,
    string? Description,
    string? Category,
    int? Stock,
    DateTime? EntryDate,
    float? Price)
{
}

