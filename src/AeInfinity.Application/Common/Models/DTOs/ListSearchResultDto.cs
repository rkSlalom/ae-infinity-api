namespace AeInfinity.Application.Common.Models.DTOs;

public class ListSearchResultDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string MatchType { get; set; } = string.Empty; // "name" or "description"
}

