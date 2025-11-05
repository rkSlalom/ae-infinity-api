using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;

namespace AeInfinity.Application.Features.ListItems.Commands.ReorderItems;

public class ReorderItemsCommandHandler : IRequestHandler<ReorderItemsCommand, Unit>
{
    private readonly IListPermissionService _permissionService;
    private readonly IListItemRepository _itemRepository;
    private readonly IRealtimeNotificationService _realtimeService;

    public ReorderItemsCommandHandler(
        IListPermissionService permissionService,
        IListItemRepository itemRepository,
        IRealtimeNotificationService realtimeService)
    {
        _permissionService = permissionService;
        _itemRepository = itemRepository;
        _realtimeService = realtimeService;
    }

    public async Task<Unit> Handle(ReorderItemsCommand request, CancellationToken cancellationToken)
    {
        // Check permissions
        var hasAccess = await _permissionService.CanUserAccessListAsync(request.UserId, request.ListId, cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        var role = await _permissionService.GetUserRoleForListAsync(request.UserId, request.ListId, cancellationToken);
        if (role == "Viewer")
        {
            throw new ForbiddenException("Viewers cannot reorder items.");
        }

        // Convert to IListItemRepository ItemPosition format
        var positions = request.Items.Select(i => new Common.Interfaces.ItemPosition
        {
            ItemId = i.ItemId,
            Position = i.Position
        }).ToList();

        // Reorder items
        await _itemRepository.ReorderAsync(positions, cancellationToken);

        // Broadcast real-time event
        var reorderedItems = request.Items.Select(i => (i.ItemId, i.Position));
        await _realtimeService.NotifyItemsReorderedAsync(request.ListId, reorderedItems);

        return Unit.Value;
    }
}

