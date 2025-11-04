using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Search.Queries.SearchGlobal;

public class SearchGlobalQuery : IRequest<SearchResultDto>
{
    public Guid UserId { get; set; }
    public string Query { get; set; } = string.Empty;
    public string Scope { get; set; } = "all"; // "all", "lists", "items"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

