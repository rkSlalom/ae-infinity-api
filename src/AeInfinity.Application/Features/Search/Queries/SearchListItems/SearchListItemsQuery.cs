using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Search.Queries.SearchListItems;

public class SearchListItemsQuery : IRequest<List<ItemSearchResultDto>>
{
    public Guid UserId { get; set; }
    public Guid ListId { get; set; }
    public string Query { get; set; } = string.Empty;
}

