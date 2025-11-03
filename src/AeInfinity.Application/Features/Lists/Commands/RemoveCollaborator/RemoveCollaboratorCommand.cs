using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.RemoveCollaborator;

/// <summary>
/// Command to remove a collaborator from a list (owner only)
/// </summary>
public class RemoveCollaboratorCommand : IRequest<Unit>
{
    public Guid ListId { get; set; }
    public Guid CollaboratorUserId { get; set; } // User to remove
    public Guid UserId { get; set; } // Current user (must be owner)
}

