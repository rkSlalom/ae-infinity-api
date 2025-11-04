using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Statistics.Queries.GetUserPurchaseHistory;

public class GetUserPurchaseHistoryQueryHandler : IRequestHandler<GetUserPurchaseHistoryQuery, List<PurchaseHistoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUserPurchaseHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseHistoryDto>> Handle(GetUserPurchaseHistoryQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ListItems
            .Where(li => !li.IsDeleted && 
                        li.IsPurchased && 
                        li.PurchasedBy == request.UserId)
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

