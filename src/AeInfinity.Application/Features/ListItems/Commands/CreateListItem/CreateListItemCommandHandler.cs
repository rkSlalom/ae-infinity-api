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
    private readonly IRealtimeNotificationService _realtimeService;

    public CreateListItemCommandHandler(
        IApplicationDbContext context, 
        IListPermissionService permissionService,
        IRealtimeNotificationService realtimeService)
    {
        _context = context;
        _permissionService = permissionService;
        _realtimeService = realtimeService;
    }

    public async Task<Guid> Handle(CreateListItemCommand request, CancellationToken cancellationToken)
    {
        // Check if user has access to this list
        var hasAccess = await _permissionService.CanUserAccessListAsync(request.UserId, request.ListId, cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        // Verify category exists (additional safety check beyond validator)
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        
        if (!categoryExists)
        {
            throw new Domain.Exceptions.ValidationException(
                "CategoryId", 
                "The specified category does not exist. Please provide a valid category ID.");
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

        // Broadcast real-time event
        // Frontend expects: { listId, item: {...}, timestamp }
        await _realtimeService.NotifyItemAddedAsync(request.ListId, new
        {
            ListId = request.ListId,
            Item = new
            {
                Id = item.Id,
                ListId = item.ListId,
                Name = item.Name,
                Quantity = item.Quantity,
                Unit = item.Unit,
                CategoryId = item.CategoryId,
                Notes = item.Notes,
                ImageUrl = item.ImageUrl,
                Position = item.Position,
                IsPurchased = item.IsPurchased,
                CreatedAt = item.CreatedAt
            },
            Timestamp = DateTime.UtcNow
        });

        return item.Id;
    }
}

