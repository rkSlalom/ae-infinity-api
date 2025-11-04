using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Statistics.Queries.GetListStats;

public class GetListStatsQuery : IRequest<ListStatsDto>
{
    public Guid UserId { get; set; }
    public Guid ListId { get; set; }
}

