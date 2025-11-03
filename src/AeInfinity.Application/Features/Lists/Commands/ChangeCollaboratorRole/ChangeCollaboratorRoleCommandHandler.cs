using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Commands.ChangeCollaboratorRole;

public class ChangeCollaboratorRoleCommandHandler : IRequestHandler<ChangeCollaboratorRoleCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public ChangeCollaboratorRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(ChangeCollaboratorRoleCommand request, CancellationToken cancellationToken)
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
            throw new ForbiddenException("Only the list owner can change collaborator roles.");
        }

        // Cannot change the owner's role
        if (request.CollaboratorUserId == list.OwnerId)
        {
            throw new ValidationException("CollaboratorUserId", "Cannot change the owner's role. Transfer ownership instead.");
        }

        // Verify new role exists
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == request.NewRoleId, cancellationToken);

        if (role == null)
        {
            throw new NotFoundException("Role", request.NewRoleId);
        }

        // Find the collaboration
        var collaboration = await _context.UserToLists
            .FirstOrDefaultAsync(utl => 
                utl.ListId == request.ListId && 
                utl.UserId == request.CollaboratorUserId, 
                cancellationToken);

        if (collaboration == null)
        {
            throw new NotFoundException($"Collaborator not found on this list.");
        }

        // Update the role
        collaboration.RoleId = request.NewRoleId;
        collaboration.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

