namespace AeInfinity.Application.Common.Models.DTOs;

public class ListItemDto
{
    public Guid Id { get; set; }
    public Guid ListId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public Guid CategoryId { get; set; }
    public CategoryDto? Category { get; set; }
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPurchased { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public Guid? PurchasedBy { get; set; }
    public UserBasicDto? PurchasedByUser { get; set; }
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public UserBasicDto? Creator { get; set; }
}

public class ListItemBasicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public bool IsPurchased { get; set; }
    public int Position { get; set; }
}

