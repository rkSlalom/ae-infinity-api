using MediatR;

namespace AeInfinity.Application.Features.Users.Notifications;

/// <summary>
/// MediatR notification for profile updates (for SignalR broadcasting)
/// </summary>
public class ProfileUpdatedNotification : INotification
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public DateTime UpdatedAt { get; init; }
}

