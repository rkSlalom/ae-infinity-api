using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Statistics.Queries.GetListActivity;

public class GetListActivityQuery : IRequest<List<ListActivityDto>>
{
    public Guid UserId { get; set; }
    public Guid ListId { get; set; }
    public int? Limit { get; set; } = 20; // Default to last 20 activities
}

