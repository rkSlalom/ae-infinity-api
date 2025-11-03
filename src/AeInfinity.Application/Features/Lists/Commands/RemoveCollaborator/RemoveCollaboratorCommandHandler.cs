using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Commands.RemoveCollaborator;

public class RemoveCollaboratorCommandHandler : IRequestHandler<RemoveCollaboratorCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public RemoveCollaboratorCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(RemoveCollaboratorCommand request, CancellationToken cancellationToken)
    {
        // Verify list exists
        var list = await _context.Lists
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        // Verify current user is the owner
        if (list.OwnerId != request.UserId)
        {
            throw new ForbiddenException("Only the list owner can remove collaborators.");
        }

        // Cannot remove the owner
        if (request.CollaboratorUserId == list.OwnerId)
        {
            throw new ValidationException("CollaboratorUserId", "Cannot remove the list owner. Transfer ownership first.");
        }

        // Find the collaboration
        var collaboration = await _context.UserToLists
            .FirstOrDefaultAsync(utl => 
                utl.ListId == request.ListId && 
                utl.UserId == request.CollaboratorUserId, 
                cancellationToken);

        if (collaboration == null)
        {
            throw new NotFoundException("Collaborator", request.CollaboratorUserId);
        }

        // Soft delete the collaboration
        collaboration.IsDeleted = true;
        collaboration.DeletedAt = DateTime.UtcNow;
        collaboration.DeletedBy = request.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

