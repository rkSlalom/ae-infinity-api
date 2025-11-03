namespace AeInfinity.Application.Common.Models.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsDefault { get; set; }
    public bool IsCustom { get; set; }
    public int SortOrder { get; set; }
}

