using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Statistics.Queries.GetUserStats;

public class GetUserStatsQuery : IRequest<UserStatsDto>
{
    public Guid UserId { get; set; }
}

