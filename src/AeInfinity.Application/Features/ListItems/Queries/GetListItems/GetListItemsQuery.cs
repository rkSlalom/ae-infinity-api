using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.ListItems.Queries.GetListItems;

public class GetListItemsQuery : IRequest<List<ListItemDto>>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsPurchased { get; set; }
    public Guid? CreatedBy { get; set; }
}

