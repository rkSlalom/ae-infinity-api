namespace AeInfinity.Application.Common.Interfaces;

/// <summary>
/// Service for checking user permissions on lists
/// </summary>
public interface IListPermissionService
{
    /// <summary>
    /// Check if user can view/access a list (owner or collaborator)
    /// </summary>
    Task<bool> CanUserAccessListAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if user can edit list details (owner only in prototype)
    /// </summary>
    Task<bool> CanUserEditListAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if user can delete list (owner only)
    /// </summary>
    Task<bool> CanUserDeleteListAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if user can archive/unarchive list (owner only)
    /// </summary>
    Task<bool> CanUserArchiveListAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get user's role for a specific list
    /// </summary>
    Task<string?> GetUserRoleForListAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if user is the owner of the list
    /// </summary>
    Task<bool> IsUserListOwnerAsync(Guid userId, Guid listId, CancellationToken cancellationToken = default);
}

