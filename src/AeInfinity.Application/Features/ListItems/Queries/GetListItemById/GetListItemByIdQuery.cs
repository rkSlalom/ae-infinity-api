using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.ListItems.Queries.GetListItemById;

public class GetListItemByIdQuery : IRequest<ListItemDto>
{
    public Guid ListId { get; set; }
    public Guid ItemId { get; set; }
    public Guid UserId { get; set; }
}

