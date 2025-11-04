using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Statistics.Queries.GetUserPurchaseHistory;

public class GetUserPurchaseHistoryQuery : IRequest<List<PurchaseHistoryDto>>
{
    public Guid UserId { get; set; }
    public int? Limit { get; set; } = 50; // Default to last 50 purchases
}

