using System.Security.Claims;
using AeInfinity.Api.Services.Realtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AeInfinity.Api.Hubs;

/// <summary>
/// SignalR hub for real-time shopping list updates
/// </summary>
[Authorize]
public class ShoppingListHub : Hub
{
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<ShoppingListHub> _logger;

    public ShoppingListHub(
        IConnectionManager connectionManager,
        ILogger<ShoppingListHub> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        var connectionId = Context.ConnectionId;

        _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, connectionId);

        await _connectionManager.AddConnectionAsync(userId, connectionId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        var connectionId = Context.ConnectionId;

        _logger.LogInformation("User {UserId} disconnected (connection {ConnectionId})", userId, connectionId);

        await _connectionManager.RemoveConnectionAsync(userId, connectionId);

        // Leave all list groups
        var listIds = await _connectionManager.GetUserListsAsync(userId);
        foreach (var listId in listIds)
        {
            await Groups.RemoveFromGroupAsync(connectionId, GetListGroupName(listId));
        }

        await _connectionManager.RemoveUserFromAllListsAsync(userId);

        if (exception != null)
        {
            _logger.LogError(exception, "User {UserId} disconnected with error", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a shopping list group to receive real-time updates
    /// </summary>
    /// <param name="listId">The list ID to join</param>
    public async Task JoinList(Guid listId)
    {
        var userId = GetUserId();
        var connectionId = Context.ConnectionId;

        _logger.LogInformation("User {UserId} joining list {ListId}", userId, listId);

        // TODO: Verify user has access to this list via permission service
        // For now, we'll assume access is granted

        var groupName = GetListGroupName(listId);
        await Groups.AddToGroupAsync(connectionId, groupName);
        await _connectionManager.AddUserToListAsync(userId, listId);

        _logger.LogInformation("User {UserId} joined list {ListId} successfully", userId, listId);

        // Notify other users in the list that this user has joined
        await Clients.OthersInGroup(groupName).SendAsync("UserJoined", new
        {
            UserId = userId,
            ListId = listId,
            JoinedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Leave a shopping list group
    /// </summary>
    /// <param name="listId">The list ID to leave</param>
    public async Task LeaveList(Guid listId)
    {
        var userId = GetUserId();
        var connectionId = Context.ConnectionId;

        _logger.LogInformation("User {UserId} leaving list {ListId}", userId, listId);

        var groupName = GetListGroupName(listId);
        await Groups.RemoveFromGroupAsync(connectionId, groupName);
        await _connectionManager.RemoveUserFromListAsync(userId, listId);

        _logger.LogInformation("User {UserId} left list {ListId} successfully", userId, listId);

        // Notify other users in the list that this user has left
        await Clients.OthersInGroup(groupName).SendAsync("UserLeft", new
        {
            UserId = userId,
            ListId = listId,
            LeftAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Update user presence on a list
    /// </summary>
    /// <param name="listId">The list ID</param>
    /// <param name="isActive">Whether the user is actively viewing</param>
    public async Task UpdatePresence(Guid listId, bool isActive)
    {
        var userId = GetUserId();

        _logger.LogDebug("User {UserId} updated presence on list {ListId}: {IsActive}", userId, listId, isActive);

        await _connectionManager.UpdatePresenceAsync(userId, listId, isActive);

        var groupName = GetListGroupName(listId);

        // Notify other users in the list about presence change
        await Clients.OthersInGroup(groupName).SendAsync("PresenceUpdate", new
        {
            UserId = userId,
            ListId = listId,
            IsActive = isActive,
            UpdatedAt = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get the current user's ID from the JWT claims
    /// </summary>
    private Guid GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new HubException("User ID not found in token");

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new HubException("Invalid user ID format");
        }

        return userId;
    }

    /// <summary>
    /// Get the SignalR group name for a list
    /// </summary>
    private static string GetListGroupName(Guid listId) => $"list-{listId}";
}

