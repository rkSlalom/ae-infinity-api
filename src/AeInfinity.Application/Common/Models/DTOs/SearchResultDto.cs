namespace AeInfinity.Application.Common.Models.DTOs;

public class SearchResultDto
{
    public List<ListSearchResultDto> Lists { get; set; } = new();
    public List<ItemSearchResultDto> Items { get; set; } = new();
    public PaginationDto Pagination { get; set; } = new();
}

