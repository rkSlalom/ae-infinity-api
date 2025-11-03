using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.AcceptInvitation;

/// <summary>
/// Command to accept a list invitation
/// </summary>
public class AcceptInvitationCommand : IRequest<Unit>
{
    public Guid InvitationId { get; set; }
    public Guid UserId { get; set; } // Current user (must match invitation user)
}

