namespace AeInfinity.Application.Common.Models.DTOs;

public class ListActivityDto
{
    public Guid Id { get; set; }
    public string ActivityType { get; set; } = string.Empty; // "item_created", "item_updated", "item_purchased", "item_deleted", "list_updated"
    public Guid UserId { get; set; }
    public string UserDisplayName { get; set; } = string.Empty;
    public string? ItemName { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

