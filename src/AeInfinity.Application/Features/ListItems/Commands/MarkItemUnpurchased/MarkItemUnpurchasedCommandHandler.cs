using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.ListItems.Commands.MarkItemUnpurchased;

public class MarkItemUnpurchasedCommandHandler : IRequestHandler<MarkItemUnpurchasedCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;
    private readonly IRealtimeNotificationService _realtimeService;

    public MarkItemUnpurchasedCommandHandler(
        IApplicationDbContext context, 
        IListPermissionService permissionService,
        IRealtimeNotificationService realtimeService)
    {
        _context = context;
        _permissionService = permissionService;
        _realtimeService = realtimeService;
    }

    public async Task<Unit> Handle(MarkItemUnpurchasedCommand request, CancellationToken cancellationToken)
    {
        // Check if user has access to this list
        var hasAccess = await _permissionService.CanUserAccessListAsync(request.UserId, request.ListId, cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        var item = await _context.ListItems
            .FirstOrDefaultAsync(li => li.Id == request.ItemId && li.ListId == request.ListId, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException("ListItem", request.ItemId);
        }

        if (!item.IsPurchased)
        {
            throw new ValidationException("Item is not marked as purchased.");
        }

        item.IsPurchased = false;
        item.PurchasedAt = null;
        item.PurchasedBy = null;

        await _context.SaveChangesAsync(cancellationToken);

        // Broadcast real-time event
        // Frontend expects: { listId, itemId, isPurchased, purchasedBy, purchasedAt, timestamp }
        await _realtimeService.NotifyItemPurchasedStatusChangedAsync(request.ListId, new
        {
            ListId = request.ListId,
            ItemId = item.Id,
            IsPurchased = false,
            PurchasedBy = (Guid?)null,
            PurchasedAt = (DateTime?)null,
            Timestamp = DateTime.UtcNow
        });

        return Unit.Value;
    }
}

