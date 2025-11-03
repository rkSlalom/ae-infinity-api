using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Queries.GetListById;

public class GetListByIdQueryHandler : IRequestHandler<GetListByIdQuery, ListDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;
    private readonly IMapper _mapper;

    public GetListByIdQueryHandler(
        IApplicationDbContext context,
        IListPermissionService permissionService,
        IMapper mapper)
    {
        _context = context;
        _permissionService = permissionService;
        _mapper = mapper;
    }

    public async Task<ListDetailDto> Handle(GetListByIdQuery request, CancellationToken cancellationToken)
    {
        // Check if user has access to this list
        var hasAccess = await _permissionService.CanUserAccessListAsync(request.UserId, request.ListId, cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        var list = await _context.Lists
            .Include(l => l.Owner)
            .Include(l => l.Items)
            .Include(l => l.Collaborators)
                .ThenInclude(c => c.User)
            .Include(l => l.Collaborators)
                .ThenInclude(c => c.Role)
            .Include(l => l.Collaborators)
                .ThenInclude(c => c.InvitedByUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        var dto = _mapper.Map<ListDetailDto>(list);
        
        // Add user's role for this list
        dto.UserRole = await _permissionService.GetUserRoleForListAsync(request.UserId, request.ListId, cancellationToken);

        return dto;
    }
}

