using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Commands.UpdateList;

public class UpdateListCommandHandler : IRequestHandler<UpdateListCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;
    private readonly IRealtimeNotificationService _realtimeService;

    public UpdateListCommandHandler(
        IApplicationDbContext context, 
        IListPermissionService permissionService,
        IRealtimeNotificationService realtimeService)
    {
        _context = context;
        _permissionService = permissionService;
        _realtimeService = realtimeService;
    }

    public async Task<Unit> Handle(UpdateListCommand request, CancellationToken cancellationToken)
    {
        // Check if user can edit this list (owner only)
        var canEdit = await _permissionService.CanUserEditListAsync(request.UserId, request.ListId, cancellationToken);
        if (!canEdit)
        {
            throw new ForbiddenException("Only the list owner can edit list details.");
        }

        var list = await _context.Lists
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        list.Name = request.Name;
        list.Description = request.Description;

        await _context.SaveChangesAsync(cancellationToken);

        // Broadcast list updated event to SignalR clients
        await _realtimeService.NotifyListUpdatedAsync(list.Id, new
        {
            Id = list.Id,
            Name = list.Name,
            Description = list.Description,
            OwnerId = list.OwnerId,
            UpdatedAt = DateTime.UtcNow
        });

        return Unit.Value;
    }
}

