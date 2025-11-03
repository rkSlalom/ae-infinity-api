using AeInfinity.Domain.Common;

namespace AeInfinity.Domain.Entities;

/// <summary>
/// Represents a product category for organizing shopping items
/// </summary>
public class Category : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public bool IsDefault { get; set; }
    public bool IsCustom { get; set; }
    public Guid? CustomOwnerId { get; set; }
    public int SortOrder { get; set; }

    // Navigation Properties
    public User? CustomOwner { get; set; }
    public ICollection<ListItem> ListItems { get; set; } = new List<ListItem>();
}

