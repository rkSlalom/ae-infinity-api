using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Commands.AcceptInvitation;

public class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public AcceptInvitationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
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
            throw new ForbiddenException("You can only accept your own invitations.");
        }

        // Verify invitation is still pending
        if (!invitation.IsPending)
        {
            throw new ValidationException("Invitation", "This invitation has already been responded to.");
        }

        // Accept the invitation
        invitation.IsPending = false;
        invitation.AcceptedAt = DateTime.UtcNow;
        invitation.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

