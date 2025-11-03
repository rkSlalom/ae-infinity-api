namespace AeInfinity.Application.Common.Models.DTOs;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool CanCreateItems { get; set; }
    public bool CanEditItems { get; set; }
    public bool CanDeleteItems { get; set; }
    public bool CanEditOwnItemsOnly { get; set; }
    public bool CanMarkPurchased { get; set; }
    public bool CanEditListDetails { get; set; }
    public bool CanManageCollaborators { get; set; }
    public bool CanDeleteList { get; set; }
    public bool CanArchiveList { get; set; }
    public int PriorityOrder { get; set; }
}

