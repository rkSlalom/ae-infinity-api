using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Commands.DeclineInvitation;

public class DeclineInvitationCommandHandler : IRequestHandler<DeclineInvitationCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeclineInvitationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeclineInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await _context.UserToLists
            .FirstOrDefaultAsync(utl => utl.Id == request.InvitationId, cancellationToken);

        if (invitation == null)
        {
            throw new NotFoundException("Invitation", request.InvitationId);
        }

        // Verify the invitation is for the current user
        if (invitation.UserId != request.UserId)
        {
            throw new ForbiddenException("You can only decline your own invitations.");
        }

        // Verify invitation is still pending
        if (!invitation.IsPending)
        {
            throw new ValidationException("Invitation", "This invitation has already been responded to.");
        }

        // Soft delete the invitation (decline)
        invitation.IsDeleted = true;
        invitation.DeletedAt = DateTime.UtcNow;
        invitation.DeletedBy = request.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

