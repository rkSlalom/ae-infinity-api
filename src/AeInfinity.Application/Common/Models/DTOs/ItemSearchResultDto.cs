namespace AeInfinity.Application.Common.Models.DTOs;

public class ItemSearchResultDto
{
    public Guid Id { get; set; }
    public Guid ListId { get; set; }
    public string ListName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string MatchType { get; set; } = string.Empty; // "name" or "notes"
}

