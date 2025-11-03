using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Lists.Queries.GetLists;

public class GetListsQuery : IRequest<List<ListDto>>
{
    public Guid UserId { get; set; }
    public bool IncludeArchived { get; set; } = false;
}

