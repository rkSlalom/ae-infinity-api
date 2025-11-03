using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Commands.LeaveList;

public class LeaveListCommandHandler : IRequestHandler<LeaveListCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public LeaveListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(LeaveListCommand request, CancellationToken cancellationToken)
    {
        // Verify list exists
        var list = await _context.Lists
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        // Owner cannot leave (must transfer ownership first)
        if (list.OwnerId == request.UserId)
        {
            throw new ValidationException("UserId", "The list owner cannot leave. Transfer ownership or delete the list.");
        }

        // Find the collaboration
        var collaboration = await _context.UserToLists
            .FirstOrDefaultAsync(utl => 
                utl.ListId == request.ListId && 
                utl.UserId == request.UserId, 
                cancellationToken);

        if (collaboration == null)
        {
            throw new NotFoundException($"You are not a collaborator on this list.");
        }

        // Soft delete the collaboration
        collaboration.IsDeleted = true;
        collaboration.DeletedAt = DateTime.UtcNow;
        collaboration.DeletedBy = request.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

