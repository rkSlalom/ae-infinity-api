using AeInfinity.Domain.Common;

namespace AeInfinity.Domain.Entities;

/// <summary>
/// Represents an individual item within a shopping list
/// </summary>
public class ListItem : BaseAuditableEntity
{
    public Guid ListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public Guid CategoryId { get; set; }
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPurchased { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public Guid? PurchasedBy { get; set; }
    public int Position { get; set; }

    // Navigation Properties
    public List List { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public User? PurchasedByUser { get; set; }
    public User? Creator { get; set; }
}

