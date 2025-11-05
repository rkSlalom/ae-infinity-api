using AeInfinity.Domain.Common;

namespace AeInfinity.Domain.Events;

/// <summary>
/// Domain event raised when a user's profile is updated
/// </summary>
public class ProfileUpdatedEvent : IDomainEvent
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

