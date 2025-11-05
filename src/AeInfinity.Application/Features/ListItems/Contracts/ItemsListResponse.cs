using AeInfinity.Application.Common.Models.DTOs;

namespace AeInfinity.Application.Features.ListItems.Contracts;

public class ItemsListResponse
{
    public List<ListItemDto> Items { get; set; } = new();
    public ItemsMetadata Metadata { get; set; } = new();
    public string Permission { get; set; } = string.Empty; // "Owner", "Editor", "Viewer"
}

public class ItemsMetadata
{
    public int TotalCount { get; set; }
    public int PurchasedCount { get; set; }
    public int UnpurchasedCount { get; set; }
    public bool AllPurchased { get; set; }
}

