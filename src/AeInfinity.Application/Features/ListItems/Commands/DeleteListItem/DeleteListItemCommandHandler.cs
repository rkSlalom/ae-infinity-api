using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.ListItems.Commands.DeleteListItem;

public class DeleteListItemCommandHandler : IRequestHandler<DeleteListItemCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;

    public DeleteListItemCommandHandler(IApplicationDbContext context, IListPermissionService permissionService)
    {
        _context = context;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteListItemCommand request, CancellationToken cancellationToken)
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

        // For prototype, we'll allow anyone with access to delete items
        // In full implementation with Editor-Limited, we'd check:
        // if (isEditorLimited && item.CreatedBy != request.UserId)
        //     throw new ForbiddenException("You can only delete items you created.");

        // Soft delete
        _context.ListItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

