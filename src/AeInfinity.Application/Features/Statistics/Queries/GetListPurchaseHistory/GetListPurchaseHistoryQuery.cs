using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Statistics.Queries.GetListPurchaseHistory;

public class GetListPurchaseHistoryQuery : IRequest<List<PurchaseHistoryDto>>
{
    public Guid UserId { get; set; }
    public Guid ListId { get; set; }
    public int? Limit { get; set; } = 50; // Default to last 50 purchases
}

