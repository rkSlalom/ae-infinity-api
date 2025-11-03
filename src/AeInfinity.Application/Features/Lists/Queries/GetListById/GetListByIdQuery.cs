using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Lists.Queries.GetListById;

public class GetListByIdQuery : IRequest<ListDetailDto>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
}

