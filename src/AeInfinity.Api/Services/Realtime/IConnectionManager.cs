namespace AeInfinity.Api.Services.Realtime;

/// <summary>
/// Manages SignalR connection tracking for users and lists
/// </summary>
public interface IConnectionManager
{
    /// <summary>
    /// Add a new connection for a user
    /// </summary>
    Task AddConnectionAsync(Guid userId, string connectionId);

    /// <summary>
    /// Remove a connection for a user
    /// </summary>
    Task RemoveConnectionAsync(Guid userId, string connectionId);

    /// <summary>
    /// Add a user to a list group
    /// </summary>
    Task AddUserToListAsync(Guid userId, Guid listId);

    /// <summary>
    /// Remove a user from a list group
    /// </summary>
    Task RemoveUserFromListAsync(Guid userId, Guid listId);

    /// <summary>
    /// Remove a user from all lists
    /// </summary>
    Task RemoveUserFromAllListsAsync(Guid userId);

    /// <summary>
    /// Get all connection IDs for a user
    /// </summary>
    Task<IEnumerable<string>> GetUserConnectionsAsync(Guid userId);

    /// <summary>
    /// Get all lists a user is currently viewing
    /// </summary>
    Task<IEnumerable<Guid>> GetUserListsAsync(Guid userId);

    /// <summary>
    /// Get all active users viewing a specific list
    /// </summary>
    Task<IEnumerable<Guid>> GetListViewersAsync(Guid listId);

    /// <summary>
    /// Update user presence on a list
    /// </summary>
    Task UpdatePresenceAsync(Guid userId, Guid listId, bool isActive);

    /// <summary>
    /// Check if a user is actively viewing a list
    /// </summary>
    Task<bool> IsUserActiveOnListAsync(Guid userId, Guid listId);
}

