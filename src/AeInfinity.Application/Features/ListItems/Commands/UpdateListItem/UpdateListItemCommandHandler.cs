using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.ListItems.Commands.UpdateListItem;

public class UpdateListItemCommandHandler : IRequestHandler<UpdateListItemCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;

    public UpdateListItemCommandHandler(IApplicationDbContext context, IListPermissionService permissionService)
    {
        _context = context;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(UpdateListItemCommand request, CancellationToken cancellationToken)
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

        // For prototype, we'll allow anyone with access to edit items
        // In full implementation with Editor-Limited, we'd check:
        // if (isEditorLimited && item.CreatedBy != request.UserId)
        //     throw new ForbiddenException("You can only edit items you created.");

        item.Name = request.Name;
        item.Quantity = request.Quantity;
        item.Unit = request.Unit;
        item.CategoryId = request.CategoryId;
        item.Notes = request.Notes;
        item.ImageUrl = request.ImageUrl;
        item.Position = request.Position;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

