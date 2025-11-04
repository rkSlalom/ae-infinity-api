using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Search.Queries.SearchGlobal;

public class SearchGlobalQueryHandler : IRequestHandler<SearchGlobalQuery, SearchResultDto>
{
    private readonly IApplicationDbContext _context;

    public SearchGlobalQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SearchResultDto> Handle(SearchGlobalQuery request, CancellationToken cancellationToken)
    {
        var result = new SearchResultDto();
        var searchTerm = $"%{request.Query}%";

        // Get accessible list IDs for the user (owned or collaborated)
        var accessibleListIds = await _context.UserToLists
            .Where(utl => utl.UserId == request.UserId && !utl.IsDeleted)
            .Select(utl => utl.ListId)
            .ToListAsync(cancellationToken);

        // Search Lists
        if (request.Scope == "all" || request.Scope == "lists")
        {
            var lists = await _context.Lists
                .Where(l => !l.IsDeleted &&
                           accessibleListIds.Contains(l.Id) &&
                           (EF.Functions.Like(l.Name, searchTerm) || 
                            EF.Functions.Like(l.Description ?? "", searchTerm)))
                .Select(l => new ListSearchResultDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Description = l.Description,
                    MatchType = EF.Functions.Like(l.Name, searchTerm) ? "name" : "description"
                })
                .ToListAsync(cancellationToken);

            result.Lists = lists;
        }

        // Search Items
        if (request.Scope == "all" || request.Scope == "items")
        {
            var items = await _context.ListItems
                .Where(li => !li.IsDeleted &&
                            accessibleListIds.Contains(li.ListId) &&
                            (EF.Functions.Like(li.Name, searchTerm) || 
                             EF.Functions.Like(li.Notes ?? "", searchTerm)))
                .Include(li => li.List)
                .Select(li => new ItemSearchResultDto
                {
                    Id = li.Id,
                    ListId = li.ListId,
                    ListName = li.List.Name,
                    Name = li.Name,
                    Notes = li.Notes,
                    MatchType = EF.Functions.Like(li.Name, searchTerm) ? "name" : "notes"
                })
                .ToListAsync(cancellationToken);

            result.Items = items;
        }

        // Calculate pagination
        var totalCount = result.Lists.Count + result.Items.Count;
        result.Pagination = new PaginationDto
        {
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };

        // Apply pagination
        var skip = (request.Page - 1) * request.PageSize;
        var combinedResults = result.Lists.Count + result.Items.Count;
        
        if (skip >= result.Lists.Count)
        {
            // Skip past all lists, take from items
            result.Lists.Clear();
            result.Items = result.Items.Skip(skip - result.Lists.Count).Take(request.PageSize).ToList();
        }
        else if (skip + request.PageSize <= result.Lists.Count)
        {
            // Take only from lists
            result.Lists = result.Lists.Skip(skip).Take(request.PageSize).ToList();
            result.Items.Clear();
        }
        else
        {
            // Take some from lists and some from items
            var listsToTake = result.Lists.Count - skip;
            result.Lists = result.Lists.Skip(skip).Take(listsToTake).ToList();
            result.Items = result.Items.Take(request.PageSize - listsToTake).ToList();
        }

        return result;
    }
}

