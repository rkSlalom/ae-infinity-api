using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Entities;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Commands.ShareList;

public class ShareListCommandHandler : IRequestHandler<ShareListCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public ShareListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(ShareListCommand request, CancellationToken cancellationToken)
    {
        // Verify list exists
        var list = await _context.Lists
            .Include(l => l.Collaborators)
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        // Verify current user is the owner
        if (list.OwnerId != request.UserId)
        {
            throw new ForbiddenException("Only the list owner can share the list.");
        }

        // Find invitee by email
        var invitee = await _context.Users
            .FirstOrDefaultAsync(u => u.EmailNormalized == request.InviteeEmail.ToUpper(), cancellationToken);

        if (invitee == null)
        {
            throw new NotFoundException("User", request.InviteeEmail);
        }

        // Cannot invite yourself
        if (invitee.Id == request.UserId)
        {
            throw new ValidationException("InviteeEmail", "You cannot invite yourself to the list.");
        }

        // Check if user is already a collaborator (active or pending)
        var existingCollaboration = await _context.UserToLists
            .FirstOrDefaultAsync(utl => 
                utl.ListId == request.ListId && 
                utl.UserId == invitee.Id, 
                cancellationToken);

        if (existingCollaboration != null)
        {
            if (existingCollaboration.IsPending)
            {
                throw new ValidationException("InviteeEmail", "This user already has a pending invitation to this list.");
            }
            else
            {
                throw new ValidationException("InviteeEmail", "This user is already a collaborator on this list.");
            }
        }

        // Verify role exists
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

        if (role == null)
        {
            throw new NotFoundException("Role", request.RoleId);
        }

        // Create invitation
        var invitation = new UserToList
        {
            Id = Guid.NewGuid(),
            ListId = request.ListId,
            UserId = invitee.Id,
            RoleId = request.RoleId,
            InvitedBy = request.UserId,
            InvitedAt = DateTime.UtcNow,
            IsPending = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserToLists.Add(invitation);
        await _context.SaveChangesAsync(cancellationToken);

        return invitation.Id;
    }
}

