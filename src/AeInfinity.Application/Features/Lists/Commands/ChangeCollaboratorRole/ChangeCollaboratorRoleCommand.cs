using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.ChangeCollaboratorRole;

/// <summary>
/// Command to change a collaborator's role (owner only)
/// </summary>
public class ChangeCollaboratorRoleCommand : IRequest<Unit>
{
    public Guid ListId { get; set; }
    public Guid CollaboratorUserId { get; set; } // User whose role to change
    public Guid NewRoleId { get; set; }
    public Guid UserId { get; set; } // Current user (must be owner)
}

