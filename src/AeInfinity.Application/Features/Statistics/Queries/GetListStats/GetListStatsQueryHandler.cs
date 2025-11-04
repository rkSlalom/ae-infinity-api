using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Statistics.Queries.GetListStats;

public class GetListStatsQueryHandler : IRequestHandler<GetListStatsQuery, ListStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetListStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ListStatsDto> Handle(GetListStatsQuery request, CancellationToken cancellationToken)
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

        // Count total items
        var totalItems = await _context.ListItems
            .Where(li => !li.IsDeleted && li.ListId == request.ListId)
            .CountAsync(cancellationToken);

        // Count purchased items
        var purchasedItems = await _context.ListItems
            .Where(li => !li.IsDeleted && li.ListId == request.ListId && li.IsPurchased)
            .CountAsync(cancellationToken);

        // Count unpurchased items
        var unpurchasedItems = totalItems - purchasedItems;

        // Count collaborators
        var totalCollaborators = await _context.UserToLists
            .Where(utl => !utl.IsDeleted && utl.ListId == request.ListId && !utl.IsPending)
            .CountAsync(cancellationToken);

        // Get last activity (last item update or purchase)
        var lastActivityAt = await _context.ListItems
            .Where(li => !li.IsDeleted && li.ListId == request.ListId)
            .MaxAsync(li => (DateTime?)li.UpdatedAt, cancellationToken);

        // If no items, use list's UpdatedAt
        if (!lastActivityAt.HasValue)
        {
            lastActivityAt = list.UpdatedAt;
        }

        return new ListStatsDto
        {
            ListId = list.Id,
            ListName = list.Name,
            TotalItems = totalItems,
            PurchasedItems = purchasedItems,
            UnpurchasedItems = unpurchasedItems,
            TotalCollaborators = totalCollaborators,
            CreatedAt = list.CreatedAt,
            LastActivityAt = lastActivityAt
        };
    }
}

