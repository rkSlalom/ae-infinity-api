using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.ListItems.Queries.GetListItems;

public class GetListItemsQueryHandler : IRequestHandler<GetListItemsQuery, List<ListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;
    private readonly IMapper _mapper;

    public GetListItemsQueryHandler(
        IApplicationDbContext context,
        IListPermissionService permissionService,
        IMapper mapper)
    {
        _context = context;
        _permissionService = permissionService;
        _mapper = mapper;
    }

    public async Task<List<ListItemDto>> Handle(GetListItemsQuery request, CancellationToken cancellationToken)
    {
        // Check if user has access to this list
        var hasAccess = await _permissionService.CanUserAccessListAsync(request.UserId, request.ListId, cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        var query = _context.ListItems
            .Include(li => li.Category)
            .Include(li => li.PurchasedByUser)
            .AsNoTracking()
            .Where(li => li.ListId == request.ListId);

        // Apply filters
        if (request.CategoryId.HasValue)
        {
            query = query.Where(li => li.CategoryId == request.CategoryId.Value);
        }

        if (request.IsPurchased.HasValue)
        {
            query = query.Where(li => li.IsPurchased == request.IsPurchased.Value);
        }

        if (request.CreatedBy.HasValue)
        {
            query = query.Where(li => li.CreatedBy == request.CreatedBy.Value);
        }

        var items = await query
            .OrderBy(li => li.Position)
            .ThenBy(li => li.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<ListItemDto>>(items);
    }
}

