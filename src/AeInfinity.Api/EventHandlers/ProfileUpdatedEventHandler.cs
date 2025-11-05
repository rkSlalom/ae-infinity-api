using AeInfinity.Api.Hubs;
using AeInfinity.Application.Features.Users.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace AeInfinity.Api.EventHandlers;

/// <summary>
/// Handler for ProfileUpdatedNotification that broadcasts profile changes via SignalR
/// </summary>
public class ProfileUpdatedEventHandler : INotificationHandler<ProfileUpdatedNotification>
{
    private readonly IHubContext<ShoppingListHub> _hubContext;
    private readonly ILogger<ProfileUpdatedEventHandler> _logger;

    public ProfileUpdatedEventHandler(
        IHubContext<ShoppingListHub> hubContext,
        ILogger<ProfileUpdatedEventHandler> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Handle(ProfileUpdatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "ProfileUpdatedEventHandler invoked for User {UserId}. Broadcasting to SignalR...",
            notification.UserId);
        
        var payload = new
        {
            userId = notification.UserId,
            displayName = notification.DisplayName,
            avatarUrl = notification.AvatarUrl,
            updatedAt = notification.UpdatedAt
        };
        
        // Broadcast to all connected clients that this user's profile has been updated
        await _hubContext.Clients.All.SendAsync(
            "ProfileUpdated",
            payload,
            cancellationToken);

        _logger.LogInformation(
            "Successfully broadcasted ProfileUpdated event for User {UserId}. Payload: {@Payload}",
            notification.UserId,
            payload);
    }
}

