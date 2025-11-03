using AeInfinity.Domain.Common;

namespace AeInfinity.Domain.Entities;

/// <summary>
/// Represents a role that defines permissions for list collaboration
/// </summary>
public class Role : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Item Permissions
    public bool CanCreateItems { get; set; }
    public bool CanEditItems { get; set; }
    public bool CanDeleteItems { get; set; }
    public bool CanEditOwnItemsOnly { get; set; }
    public bool CanMarkPurchased { get; set; }
    
    // List Permissions
    public bool CanEditListDetails { get; set; }
    public bool CanManageCollaborators { get; set; }
    public bool CanDeleteList { get; set; }
    public bool CanArchiveList { get; set; }
    
    // Display
    public int PriorityOrder { get; set; }

    // Navigation Properties
    public ICollection<UserToList> UserToLists { get; set; } = new List<UserToList>();
}

