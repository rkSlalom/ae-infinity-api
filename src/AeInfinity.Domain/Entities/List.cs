using AeInfinity.Domain.Common;

namespace AeInfinity.Domain.Entities;

/// <summary>
/// Represents a shopping list
/// </summary>
public class List : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OwnerId { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public Guid? ArchivedBy { get; set; }

    // Navigation Properties
    public User Owner { get; set; } = null!;
    public ICollection<ListItem> Items { get; set; } = new List<ListItem>();
    public ICollection<UserToList> Collaborators { get; set; } = new List<UserToList>();
}

