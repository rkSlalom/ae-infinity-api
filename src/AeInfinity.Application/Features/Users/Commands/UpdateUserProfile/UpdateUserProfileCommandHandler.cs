using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Features.Users.Notifications;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AeInfinity.Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

    public UpdateUserProfileCommandHandler(
        IApplicationDbContext context, 
        IMediator mediator,
        ILogger<UpdateUserProfileCommandHandler> logger)
    {
        _context = context;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning(
                "Profile update failed: User {UserId} not found",
                request.UserId);
            throw new NotFoundException("User", request.UserId);
        }

        // Track changes for logging
        var oldDisplayName = user.DisplayName;
        var oldAvatarUrl = user.AvatarUrl;
        var displayNameChanged = oldDisplayName != request.DisplayName.Trim();
        var avatarUrlChanged = oldAvatarUrl != (string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim());

        // Update profile fields
        user.DisplayName = request.DisplayName.Trim();
        user.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) 
            ? null 
            : request.AvatarUrl.Trim();

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            // Structured logging with all relevant details
            _logger.LogInformation(
                "Profile updated successfully for User {UserId}. " +
                "DisplayName changed: {DisplayNameChanged} ('{OldName}' -> '{NewName}'), " +
                "AvatarUrl changed: {AvatarUrlChanged} ('{OldUrl}' -> '{NewUrl}')",
                user.Id,
                displayNameChanged,
                displayNameChanged ? oldDisplayName : "unchanged",
                displayNameChanged ? user.DisplayName : "unchanged",
                avatarUrlChanged,
                avatarUrlChanged ? (oldAvatarUrl ?? "null") : "unchanged",
                avatarUrlChanged ? (user.AvatarUrl ?? "null") : "unchanged");

            // Publish notification for SignalR broadcast
            await _mediator.Publish(new ProfileUpdatedNotification
            {
                UserId = user.Id,
                DisplayName = user.DisplayName,
                AvatarUrl = user.AvatarUrl,
                UpdatedAt = DateTime.UtcNow
            }, cancellationToken);

            _logger.LogDebug(
                "ProfileUpdated notification published for User {UserId}",
                user.Id);

            return Unit.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Profile update failed for User {UserId}. DisplayName: '{DisplayName}', AvatarUrl: '{AvatarUrl}'",
                user.Id,
                request.DisplayName,
                request.AvatarUrl ?? "null");
            throw;
        }
    }
}
