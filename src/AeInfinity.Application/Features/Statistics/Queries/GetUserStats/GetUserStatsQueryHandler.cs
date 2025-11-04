using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Statistics.Queries.GetUserStats;

public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, UserStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetUserStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserStatsDto> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
    {
        // Count lists owned by user
        var totalListsOwned = await _context.Lists
            .Where(l => !l.IsDeleted && l.OwnerId == request.UserId)
            .CountAsync(cancellationToken);

        // Count lists shared with user (collaborator but not owner)
        var totalListsShared = await _context.UserToLists
            .Where(utl => !utl.IsDeleted && 
                         utl.UserId == request.UserId && 
                         !utl.IsPending)
            .Join(_context.Lists,
                utl => utl.ListId,
                l => l.Id,
                (utl, l) => new { utl, l })
            .Where(x => !x.l.IsDeleted && x.l.OwnerId != request.UserId)
            .CountAsync(cancellationToken);

        // Count items created by user
        var totalItemsCreated = await _context.ListItems
            .Where(li => !li.IsDeleted && li.CreatedBy == request.UserId)
            .CountAsync(cancellationToken);

        // Count items purchased by user
        var totalItemsPurchased = await _context.ListItems
            .Where(li => !li.IsDeleted && 
                        li.IsPurchased && 
                        li.PurchasedBy == request.UserId)
            .CountAsync(cancellationToken);

        // Count active collaborations (excluding owned lists)
        var totalActiveCollaborations = await _context.UserToLists
            .Where(utl => !utl.IsDeleted && 
                         utl.UserId == request.UserId && 
                         !utl.IsPending)
            .Join(_context.Lists,
                utl => utl.ListId,
                l => l.Id,
                (utl, l) => new { utl, l })
            .Where(x => !x.l.IsDeleted && x.l.OwnerId != request.UserId)
            .CountAsync(cancellationToken);

        // Get last activity timestamp (latest of: list created, item created, item updated, item purchased)
        var lastListActivity = await _context.Lists
            .Where(l => !l.IsDeleted && l.OwnerId == request.UserId)
            .MaxAsync(l => (DateTime?)l.UpdatedAt, cancellationToken);

        var lastItemActivity = await _context.ListItems
            .Where(li => !li.IsDeleted && 
                        (li.CreatedBy == request.UserId || li.UpdatedBy == request.UserId))
            .MaxAsync(li => (DateTime?)li.UpdatedAt, cancellationToken);

        var lastPurchaseActivity = await _context.ListItems
            .Where(li => !li.IsDeleted && 
                        li.IsPurchased && 
                        li.PurchasedBy == request.UserId)
            .MaxAsync(li => (DateTime?)li.PurchasedAt, cancellationToken);

        var lastActivityAt = new[] { lastListActivity, lastItemActivity, lastPurchaseActivity }
            .Where(d => d.HasValue)
            .Max();

        return new UserStatsDto
        {
            TotalListsOwned = totalListsOwned,
            TotalListsShared = totalListsShared,
            TotalItemsCreated = totalItemsCreated,
            TotalItemsPurchased = totalItemsPurchased,
            TotalActiveCollaborations = totalActiveCollaborations,
            LastActivityAt = lastActivityAt
        };
    }
}

