namespace AeInfinity.Application.Common.Interfaces;

/// <summary>
/// Service for broadcasting real-time notifications to connected clients
/// Interface defined in Application layer, implemented in API/Infrastructure layer
/// </summary>
public interface IRealtimeNotificationService
{
    // Item Events
    Task NotifyItemAddedAsync<T>(Guid listId, T itemData);
    Task NotifyItemUpdatedAsync<T>(Guid listId, T itemData);
    Task NotifyItemDeletedAsync<T>(Guid listId, T eventData);
    Task NotifyItemPurchasedStatusChangedAsync<T>(Guid listId, T eventData);

    // List Events
    Task NotifyListUpdatedAsync(Guid listId, object listData);
    Task NotifyListArchivedAsync(Guid listId, bool isArchived);
    Task NotifyItemsReorderedAsync(Guid listId, IEnumerable<(Guid ItemId, int Position)> reorderedItems);

    // Collaboration Events
    Task NotifyCollaboratorAddedAsync(Guid listId, object collaboratorData);
    Task NotifyCollaboratorRemovedAsync(Guid listId, Guid userId);
    Task NotifyCollaboratorPermissionChangedAsync(Guid listId, Guid userId, string newPermission);
}

