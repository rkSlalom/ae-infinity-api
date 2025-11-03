using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Domain.Entities;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.ListItems.Commands.CreateListItem;

public class CreateListItemCommandHandler : IRequestHandler<CreateListItemCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;

    public CreateListItemCommandHandler(IApplicationDbContext context, IListPermissionService permissionService)
    {
        _context = context;
        _permissionService = permissionService;
    }

    public async Task<Guid> Handle(CreateListItemCommand request, CancellationToken cancellationToken)
    {
        // Check if user has access to this list
        var hasAccess = await _permissionService.CanUserAccessListAsync(request.UserId, request.ListId, cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        // For prototype, we'll allow anyone with access to create items
        // In full implementation, we'd check CanCreateItems permission from role

        // Get next position
        var maxPosition = await _context.ListItems
            .Where(li => li.ListId == request.ListId)
            .MaxAsync(li => (int?)li.Position, cancellationToken) ?? 0;

        var item = new ListItem
        {
            Id = Guid.NewGuid(),
            ListId = request.ListId,
            Name = request.Name,
            Quantity = request.Quantity,
            Unit = request.Unit,
            CategoryId = request.CategoryId,
            Notes = request.Notes,
            ImageUrl = request.ImageUrl,
            Position = maxPosition + 1
            // CreatedBy and CreatedAt will be set by SaveChangesAsync override
        };

        _context.ListItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

        return item.Id;
    }
}

