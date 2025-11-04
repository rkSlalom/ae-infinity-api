namespace AeInfinity.Application.Common.Models.DTOs;

public class PurchaseHistoryDto
{
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public Guid ListId { get; set; }
    public string ListName { get; set; } = string.Empty;
    public Guid PurchasedBy { get; set; }
    public string PurchasedByDisplayName { get; set; } = string.Empty;
    public DateTime PurchasedAt { get; set; }
    public string? CategoryName { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
}

