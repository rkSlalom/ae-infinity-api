using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Entities;
using AeInfinity.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.ListItems.Commands.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, ListItemDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;
    private readonly IListItemRepository _itemRepository;
    private readonly IRealtimeNotificationService _realtimeService;
    private readonly IMapper _mapper;

    public CreateItemCommandHandler(
        IApplicationDbContext context,
        IListPermissionService permissionService,
        IListItemRepository itemRepository,
        IRealtimeNotificationService realtimeService,
        IMapper mapper)
    {
        _context = context;
        _permissionService = permissionService;
        _itemRepository = itemRepository;
        _realtimeService = realtimeService;
        _mapper = mapper;
    }

    public async Task<ListItemDto> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        // Check permissions
        var hasAccess = await _permissionService.CanUserAccessListAsync(request.UserId, request.ListId, cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        var role = await _permissionService.GetUserRoleForListAsync(request.UserId, request.ListId, cancellationToken);
        if (role == "Viewer")
        {
            throw new ForbiddenException("Viewers cannot add items.");
        }

        // Verify list exists
        var list = await _context.Lists.FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);
        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        // Get next position
        var nextPosition = await _itemRepository.GetNextPositionAsync(request.ListId, cancellationToken);

        // Create item
        var item = new ListItem
        {
            Id = Guid.NewGuid(),
            ListId = request.ListId,
            Name = request.Name,
            Quantity = request.Quantity,
            Unit = request.Unit,
            CategoryId = request.CategoryId,
            Notes = request.Notes,
            ImageUrl = request.ImageUrl,
            Position = nextPosition,
            IsPurchased = false,
            CreatedBy = request.UserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _itemRepository.CreateAsync(item, cancellationToken);

        // Load navigation properties for response
        var createdItem = await _context.ListItems
            .Include(i => i.Category)
            .Include(i => i.Creator)
            .FirstAsync(i => i.Id == item.Id, cancellationToken);

        var result = _mapper.Map<ListItemDto>(createdItem);

        // Broadcast real-time event
        await _realtimeService.NotifyItemAddedAsync(request.ListId, new
        {
            ListId = request.ListId,
            Item = result,
            UserId = request.UserId,
            Timestamp = DateTime.UtcNow
        });

        return result;
    }
}

