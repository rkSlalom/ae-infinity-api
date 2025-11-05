using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Commands.DeleteList;

public class DeleteListCommandHandler : IRequestHandler<DeleteListCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;
    private readonly IRealtimeNotificationService _realtimeService;

    public DeleteListCommandHandler(
        IApplicationDbContext context, 
        IListPermissionService permissionService,
        IRealtimeNotificationService realtimeService)
    {
        _context = context;
        _permissionService = permissionService;
        _realtimeService = realtimeService;
    }

    public async Task<Unit> Handle(DeleteListCommand request, CancellationToken cancellationToken)
    {
        // Check if user can delete this list (owner only)
        var canDelete = await _permissionService.CanUserDeleteListAsync(request.UserId, request.ListId, cancellationToken);
        if (!canDelete)
        {
            throw new ForbiddenException("Only the list owner can delete the list.");
        }

        var list = await _context.Lists
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        var listId = list.Id;

        // Soft delete
        _context.Lists.Remove(list);
        await _context.SaveChangesAsync(cancellationToken);

        // Broadcast list deleted event to SignalR clients
        await _realtimeService.NotifyListDeletedAsync(listId);

        return Unit.Value;
    }
}

