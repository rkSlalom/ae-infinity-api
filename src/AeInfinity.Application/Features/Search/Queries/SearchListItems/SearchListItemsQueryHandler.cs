using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Search.Queries.SearchListItems;

public class SearchListItemsQueryHandler : IRequestHandler<SearchListItemsQuery, List<ItemSearchResultDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchListItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ItemSearchResultDto>> Handle(SearchListItemsQuery request, CancellationToken cancellationToken)
    {
        // Verify list exists
        var list = await _context.Lists
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        // Check user has access to this list
        var hasAccess = await _context.UserToLists
            .AnyAsync(utl => utl.ListId == request.ListId && 
                            utl.UserId == request.UserId && 
                            !utl.IsDeleted, 
                     cancellationToken);

        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        // Search items in this list
        var searchTerm = $"%{request.Query}%";
        var items = await _context.ListItems
            .Where(li => !li.IsDeleted &&
                        li.ListId == request.ListId &&
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

        return items;
    }
}

