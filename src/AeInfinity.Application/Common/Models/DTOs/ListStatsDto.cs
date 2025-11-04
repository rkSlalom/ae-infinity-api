namespace AeInfinity.Application.Common.Models.DTOs;

public class ListStatsDto
{
    public Guid ListId { get; set; }
    public string ListName { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int PurchasedItems { get; set; }
    public int UnpurchasedItems { get; set; }
    public int TotalCollaborators { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
}

