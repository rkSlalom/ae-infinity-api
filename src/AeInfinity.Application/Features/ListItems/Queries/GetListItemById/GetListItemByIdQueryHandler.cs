using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.ListItems.Queries.GetListItemById;

public class GetListItemByIdQueryHandler : IRequestHandler<GetListItemByIdQuery, ListItemDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;
    private readonly IMapper _mapper;

    public GetListItemByIdQueryHandler(
        IApplicationDbContext context,
        IListPermissionService permissionService,
        IMapper mapper)
    {
        _context = context;
        _permissionService = permissionService;
        _mapper = mapper;
    }

    public async Task<ListItemDto> Handle(GetListItemByIdQuery request, CancellationToken cancellationToken)
    {
        // Check if user has access to this list
        var hasAccess = await _permissionService.CanUserAccessListAsync(request.UserId, request.ListId, cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        var item = await _context.ListItems
            .Include(li => li.Category)
            .Include(li => li.PurchasedByUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(li => li.Id == request.ItemId && li.ListId == request.ListId, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException("ListItem", request.ItemId);
        }

        return _mapper.Map<ListItemDto>(item);
    }
}

