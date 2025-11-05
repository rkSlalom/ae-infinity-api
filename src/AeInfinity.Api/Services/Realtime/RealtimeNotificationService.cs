using AeInfinity.Api.Hubs;
using AeInfinity.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AeInfinity.Api.Services.Realtime;

/// <summary>
/// Implementation of realtime notification service using SignalR
/// </summary>
public class RealtimeNotificationService : IRealtimeNotificationService
{
    private readonly IHubContext<ShoppingListHub> _hubContext;
    private readonly ILogger<RealtimeNotificationService> _logger;

    public RealtimeNotificationService(
        IHubContext<ShoppingListHub> hubContext,
        ILogger<RealtimeNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyItemAddedAsync<T>(Guid listId, T eventData)
    {
        _logger.LogInformation("Broadcasting ItemAdded event for list {ListId}", listId);

        // Pass through the complete event data (already has ListId, Item, Timestamp)
        await _hubContext.Clients
            .Group(GetListGroupName(listId))
            .SendAsync("ItemAdded", eventData);
    }

    public async Task NotifyItemUpdatedAsync<T>(Guid listId, T eventData)
    {
        _logger.LogInformation("Broadcasting ItemUpdated event for list {ListId}", listId);

        // Pass through the complete event data (already has ListId, Item, Timestamp)
        await _hubContext.Clients
            .Group(GetListGroupName(listId))
            .SendAsync("ItemUpdated", eventData);
    }

    public async Task NotifyItemDeletedAsync<T>(Guid listId, T eventData)
    {
        _logger.LogInformation("Broadcasting ItemDeleted event for list {ListId}", listId);

        await _hubContext.Clients
            .Group(GetListGroupName(listId))
            .SendAsync("ItemDeleted", eventData);
    }

    public async Task NotifyItemPurchasedStatusChangedAsync<T>(Guid listId, T eventData)
    {
        _logger.LogInformation("Broadcasting ItemPurchasedStatusChanged event for list {ListId}", listId);

        await _hubContext.Clients
            .Group(GetListGroupName(listId))
            .SendAsync("ItemPurchasedStatusChanged", eventData);
    }

    public async Task NotifyListCreatedAsync(Guid listId, object listData)
    {
        _logger.LogInformation("Broadcasting ListCreated event for list {ListId} to all connected users", listId);

        // Broadcast to ALL connected users (dashboard-level event)
        await _hubContext.Clients
            .All
            .SendAsync("ListCreated", new
            {
                ListId = listId,
                List = listData,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task NotifyListUpdatedAsync(Guid listId, object listData)
    {
        _logger.LogInformation("Broadcasting ListUpdated event for list {ListId} to all connected users", listId);

        // Broadcast to ALL connected users (dashboard-level event)
        await _hubContext.Clients
            .All
            .SendAsync("ListUpdated", new
            {
                ListId = listId,
                List = listData,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task NotifyListDeletedAsync(Guid listId)
    {
        _logger.LogInformation("Broadcasting ListDeleted event for list {ListId} to all connected users", listId);

        // Broadcast to ALL connected users (dashboard-level event)
        await _hubContext.Clients
            .All
            .SendAsync("ListDeleted", new
            {
                ListId = listId,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task NotifyListArchivedAsync(Guid listId, bool isArchived)
    {
        _logger.LogInformation("Broadcasting ListArchived event for list {ListId}, archived: {IsArchived} to all connected users", 
            listId, isArchived);

        // Broadcast to ALL connected users (dashboard-level event)
        await _hubContext.Clients
            .All
            .SendAsync("ListArchived", new
            {
                ListId = listId,
                IsArchived = isArchived,
                ArchivedAt = isArchived ? DateTime.UtcNow : (DateTime?)null,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task NotifyItemsReorderedAsync(Guid listId, IEnumerable<(Guid ItemId, int Position)> reorderedItems)
    {
        _logger.LogInformation("Broadcasting ItemsReordered event for list {ListId}", listId);

        await _hubContext.Clients
            .Group(GetListGroupName(listId))
            .SendAsync("ItemsReordered", new
            {
                ListId = listId,
                ReorderedItems = reorderedItems.Select(x => new { ItemId = x.ItemId, Position = x.Position }),
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task NotifyCollaboratorAddedAsync(Guid listId, object collaboratorData)
    {
        _logger.LogInformation("Broadcasting CollaboratorAdded event for list {ListId}", listId);

        await _hubContext.Clients
            .Group(GetListGroupName(listId))
            .SendAsync("CollaboratorAdded", new
            {
                ListId = listId,
                Collaborator = collaboratorData,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task NotifyCollaboratorRemovedAsync(Guid listId, Guid userId)
    {
        _logger.LogInformation("Broadcasting CollaboratorRemoved event for list {ListId}, user {UserId}", 
            listId, userId);

        await _hubContext.Clients
            .Group(GetListGroupName(listId))
            .SendAsync("CollaboratorRemoved", new
            {
                ListId = listId,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            });
    }

    public async Task NotifyCollaboratorPermissionChangedAsync(Guid listId, Guid userId, string newPermission)
    {
        _logger.LogInformation("Broadcasting CollaboratorPermissionChanged event for list {ListId}, user {UserId}", 
            listId, userId);

        await _hubContext.Clients
            .Group(GetListGroupName(listId))
            .SendAsync("CollaboratorPermissionChanged", new
            {
                ListId = listId,
                UserId = userId,
                NewPermission = newPermission,
                Timestamp = DateTime.UtcNow
            });
    }

    private static string GetListGroupName(Guid listId) => $"list-{listId}";
}

