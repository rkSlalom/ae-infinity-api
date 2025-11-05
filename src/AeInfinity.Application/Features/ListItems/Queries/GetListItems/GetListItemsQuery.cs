using AeInfinity.Application.Features.ListItems.Contracts;
using MediatR;

namespace AeInfinity.Application.Features.ListItems.Queries.GetListItems;

public class GetListItemsQuery : IRequest<ItemsListResponse>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsPurchased { get; set; }
    public Guid? CreatedBy { get; set; }
}

