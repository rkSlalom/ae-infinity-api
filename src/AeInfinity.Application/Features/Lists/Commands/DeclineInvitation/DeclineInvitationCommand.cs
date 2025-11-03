using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.DeclineInvitation;

/// <summary>
/// Command to decline a list invitation
/// </summary>
public class DeclineInvitationCommand : IRequest<Unit>
{
    public Guid InvitationId { get; set; }
    public Guid UserId { get; set; } // Current user (must match invitation user)
}

