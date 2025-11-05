using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Statistics.Queries.GetListActivity;

public class GetListActivityQueryHandler : IRequestHandler<GetListActivityQuery, List<ListActivityDto>>
{
    private readonly IApplicationDbContext _context;

    public GetListActivityQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ListActivityDto>> Handle(GetListActivityQuery request, CancellationToken cancellationToken)
    {
        // Verify list exists
        var list = await _context.Lists
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        // Check user has access to this list
        var hasAccess = await _context.UserToLists
            .AnyAsync(utl => utl.ListId == request.ListId && 
                            utl.UserId == request.UserId && 
                            !utl.IsDeleted, 
                     cancellationToken);

        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        var activities = new List<ListActivityDto>();

        // Get item creations
        var itemCreations = await _context.ListItems
            .Where(li => li.ListId == request.ListId && li.CreatedBy.HasValue)
            .Include(li => li.Creator)
            .Select(li => new ListActivityDto
            {
                Id = li.Id,
                ActivityType = "item_created",
                UserId = li.CreatedBy!.Value,
                UserDisplayName = li.Creator!.DisplayName,
                ItemName = li.Name,
                Description = $"Added {li.Name} to the list",
                Timestamp = li.CreatedAt
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(itemCreations);

        // Get item updates (where UpdatedBy != CreatedBy and UpdatedAt > CreatedAt)
        var itemUpdates = await _context.ListItems
            .Where(li => li.ListId == request.ListId && 
                        li.UpdatedBy.HasValue && 
                        li.UpdatedBy != li.CreatedBy)
            .Join(_context.Users,
                li => li.UpdatedBy!.Value,
                u => u.Id,
                (li, u) => new { Item = li, User = u })
            .Select(x => new ListActivityDto
            {
                Id = x.Item.Id,
                ActivityType = "item_updated",
                UserId = x.Item.UpdatedBy!.Value,
                UserDisplayName = x.User.DisplayName,
                ItemName = x.Item.Name,
                Description = $"Updated {x.Item.Name}",
                Timestamp = x.Item.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(itemUpdates);

        // Get item purchases
        var itemPurchases = await _context.ListItems
            .Where(li => li.ListId == request.ListId && 
                        li.IsPurchased && 
                        li.PurchasedBy.HasValue)
            .Include(li => li.PurchasedByUser)
            .Select(li => new ListActivityDto
            {
                Id = li.Id,
                ActivityType = "item_purchased",
                UserId = li.PurchasedBy!.Value,
                UserDisplayName = li.PurchasedByUser!.DisplayName,
                ItemName = li.Name,
                Description = $"Purchased {li.Name}",
                Timestamp = li.PurchasedAt!.Value
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(itemPurchases);

        // Get deleted items (soft deleted)
        var itemDeletions = await _context.ListItems
            .IgnoreQueryFilters()
            .Where(li => li.ListId == request.ListId && 
                        li.IsDeleted && 
                        li.DeletedById.HasValue)
            .Join(_context.Users,
                li => li.DeletedById!.Value,
                u => u.Id,
                (li, u) => new { Item = li, User = u })
            .Select(x => new ListActivityDto
            {
                Id = x.Item.Id,
                ActivityType = "item_deleted",
                UserId = x.Item.DeletedById!.Value,
                UserDisplayName = x.User.DisplayName,
                ItemName = x.Item.Name,
                Description = $"Removed {x.Item.Name} from the list",
                Timestamp = x.Item.DeletedAt!.Value
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(itemDeletions);

        // Sort by timestamp descending and apply limit
        var sortedActivities = activities
            .OrderByDescending(a => a.Timestamp)
            .Take(request.Limit ?? 20)
            .ToList();

        return sortedActivities;
    }
}

