using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.ListItems.Contracts;
using AeInfinity.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.ListItems.Queries.GetListItems;

public class GetListItemsQueryHandler : IRequestHandler<GetListItemsQuery, ItemsListResponse>
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

    public async Task<ItemsListResponse> Handle(GetListItemsQuery request, CancellationToken cancellationToken)
    {
        // Check if user has access to this list
        var hasAccess = await _permissionService.CanUserAccessListAsync(request.UserId, request.ListId, cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        // Get user's permission level
        var permission = await _permissionService.GetUserRoleForListAsync(request.UserId, request.ListId, cancellationToken);

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

        var itemDtos = _mapper.Map<List<ListItemDto>>(items);

        // Calculate metadata
        var totalCount = items.Count;
        var purchasedCount = items.Count(i => i.IsPurchased);
        var unpurchasedCount = totalCount - purchasedCount;

        return new ItemsListResponse
        {
            Items = itemDtos,
            Metadata = new ItemsMetadata
            {
                TotalCount = totalCount,
                PurchasedCount = purchasedCount,
                UnpurchasedCount = unpurchasedCount,
                AllPurchased = totalCount > 0 && purchasedCount == totalCount
            },
            Permission = permission ?? "Viewer"
        };
    }
}

