using System.Collections.Concurrent;

namespace AeInfinity.Api.Services.Realtime;

/// <summary>
/// In-memory implementation of connection tracking
/// For production with multiple servers, use Redis or a distributed cache
/// </summary>
public class ConnectionManager : IConnectionManager
{
    // userId -> HashSet of connectionIds
    private readonly ConcurrentDictionary<Guid, HashSet<string>> _userConnections = new();

    // userId -> HashSet of listIds
    private readonly ConcurrentDictionary<Guid, HashSet<Guid>> _userLists = new();

    // listId -> HashSet of userIds
    private readonly ConcurrentDictionary<Guid, HashSet<Guid>> _listViewers = new();

    // (userId, listId) -> isActive
    private readonly ConcurrentDictionary<(Guid UserId, Guid ListId), bool> _presenceStatus = new();

    private readonly ILogger<ConnectionManager> _logger;

    public ConnectionManager(ILogger<ConnectionManager> logger)
    {
        _logger = logger;
    }

    public Task AddConnectionAsync(Guid userId, string connectionId)
    {
        _userConnections.AddOrUpdate(
            userId,
            _ =>
            {
                _logger.LogDebug("Creating new connection set for user {UserId}", userId);
                return new HashSet<string> { connectionId };
            },
            (_, connections) =>
            {
                connections.Add(connectionId);
                _logger.LogDebug("Added connection {ConnectionId} for user {UserId}. Total: {Count}", 
                    connectionId, userId, connections.Count);
                return connections;
            });

        return Task.CompletedTask;
    }

    public Task RemoveConnectionAsync(Guid userId, string connectionId)
    {
        if (_userConnections.TryGetValue(userId, out var connections))
        {
            connections.Remove(connectionId);
            _logger.LogDebug("Removed connection {ConnectionId} for user {UserId}. Remaining: {Count}",
                connectionId, userId, connections.Count);

            if (connections.Count == 0)
            {
                _userConnections.TryRemove(userId, out _);
                _logger.LogDebug("User {UserId} has no more connections", userId);
            }
        }

        return Task.CompletedTask;
    }

    public Task AddUserToListAsync(Guid userId, Guid listId)
    {
        // Add list to user's lists
        _userLists.AddOrUpdate(
            userId,
            _ => new HashSet<Guid> { listId },
            (_, lists) =>
            {
                lists.Add(listId);
                return lists;
            });

        // Add user to list's viewers
        _listViewers.AddOrUpdate(
            listId,
            _ => new HashSet<Guid> { userId },
            (_, viewers) =>
            {
                viewers.Add(userId);
                return viewers;
            });

        // Set initial presence as active
        _presenceStatus[(userId, listId)] = true;

        _logger.LogDebug("User {UserId} added to list {ListId}", userId, listId);

        return Task.CompletedTask;
    }

    public Task RemoveUserFromListAsync(Guid userId, Guid listId)
    {
        // Remove list from user's lists
        if (_userLists.TryGetValue(userId, out var userListsSet))
        {
            userListsSet.Remove(listId);
            if (userListsSet.Count == 0)
            {
                _userLists.TryRemove(userId, out _);
            }
        }

        // Remove user from list's viewers
        if (_listViewers.TryGetValue(listId, out var viewersSet))
        {
            viewersSet.Remove(userId);
            if (viewersSet.Count == 0)
            {
                _listViewers.TryRemove(listId, out _);
            }
        }

        // Remove presence status
        _presenceStatus.TryRemove((userId, listId), out _);

        _logger.LogDebug("User {UserId} removed from list {ListId}", userId, listId);

        return Task.CompletedTask;
    }

    public Task RemoveUserFromAllListsAsync(Guid userId)
    {
        if (_userLists.TryRemove(userId, out var userListsSet))
        {
            foreach (var listId in userListsSet)
            {
                // Remove user from each list's viewers
                if (_listViewers.TryGetValue(listId, out var viewersSet))
                {
                    viewersSet.Remove(userId);
                    if (viewersSet.Count == 0)
                    {
                        _listViewers.TryRemove(listId, out _);
                    }
                }

                // Remove presence status
                _presenceStatus.TryRemove((userId, listId), out _);
            }

            _logger.LogDebug("User {UserId} removed from {Count} lists", userId, userListsSet.Count);
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> GetUserConnectionsAsync(Guid userId)
    {
        var connections = _userConnections.TryGetValue(userId, out var connectionsSet)
            ? connectionsSet.ToList()
            : Enumerable.Empty<string>();

        return Task.FromResult(connections);
    }

    public Task<IEnumerable<Guid>> GetUserListsAsync(Guid userId)
    {
        var lists = _userLists.TryGetValue(userId, out var listsSet)
            ? listsSet.ToList()
            : Enumerable.Empty<Guid>();

        return Task.FromResult(lists);
    }

    public Task<IEnumerable<Guid>> GetListViewersAsync(Guid listId)
    {
        var viewers = _listViewers.TryGetValue(listId, out var viewersSet)
            ? viewersSet.ToList()
            : Enumerable.Empty<Guid>();

        return Task.FromResult(viewers);
    }

    public Task UpdatePresenceAsync(Guid userId, Guid listId, bool isActive)
    {
        _presenceStatus[(userId, listId)] = isActive;
        _logger.LogDebug("User {UserId} presence on list {ListId} set to {IsActive}", 
            userId, listId, isActive);

        return Task.CompletedTask;
    }

    public Task<bool> IsUserActiveOnListAsync(Guid userId, Guid listId)
    {
        var isActive = _presenceStatus.TryGetValue((userId, listId), out var active) && active;
        return Task.FromResult(isActive);
    }
}

