using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Statistics.Queries.GetListPurchaseHistory;

public class GetListPurchaseHistoryQueryHandler : IRequestHandler<GetListPurchaseHistoryQuery, List<PurchaseHistoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetListPurchaseHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseHistoryDto>> Handle(GetListPurchaseHistoryQuery request, CancellationToken cancellationToken)
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

        // Get purchase history for this list
        var query = _context.ListItems
            .Where(li => !li.IsDeleted && 
                        li.ListId == request.ListId &&
                        li.IsPurchased)
            .Include(li => li.List)
            .Include(li => li.Category)
            .Include(li => li.PurchasedByUser)
            .OrderByDescending(li => li.PurchasedAt)
            .AsQueryable();

        if (request.Limit.HasValue)
        {
            query = query.Take(request.Limit.Value);
        }

        var history = await query
            .Select(li => new PurchaseHistoryDto
            {
                ItemId = li.Id,
                ItemName = li.Name,
                ListId = li.ListId,
                ListName = li.List.Name,
                PurchasedBy = li.PurchasedBy!.Value,
                PurchasedByDisplayName = li.PurchasedByUser!.DisplayName,
                PurchasedAt = li.PurchasedAt!.Value,
                CategoryName = li.Category != null ? li.Category.Name : null,
                Quantity = li.Quantity,
                Unit = li.Unit
            })
            .ToListAsync(cancellationToken);

        return history;
    }
}

