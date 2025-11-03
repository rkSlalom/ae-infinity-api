using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Queries.GetListCollaborators;

public class GetListCollaboratorsQueryHandler : IRequestHandler<GetListCollaboratorsQuery, List<CollaboratorDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IListPermissionService _permissionService;
    private readonly IMapper _mapper;

    public GetListCollaboratorsQueryHandler(
        IApplicationDbContext context, 
        IListPermissionService permissionService,
        IMapper mapper)
    {
        _context = context;
        _permissionService = permissionService;
        _mapper = mapper;
    }

    public async Task<List<CollaboratorDto>> Handle(GetListCollaboratorsQuery request, CancellationToken cancellationToken)
    {
        // Verify user has access to this list
        var hasAccess = await _permissionService.CanUserAccessListAsync(request.UserId, request.ListId, cancellationToken);
        if (!hasAccess)
        {
            throw new ForbiddenException("You do not have access to this list.");
        }

        var collaborators = await _context.UserToLists
            .Include(utl => utl.User)
            .Include(utl => utl.Role)
            .Include(utl => utl.InvitedByUser)
            .AsNoTracking()
            .Where(utl => utl.ListId == request.ListId)
            .OrderBy(utl => utl.IsPending)
            .ThenBy(utl => utl.User.DisplayName)
            .ProjectTo<CollaboratorDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return collaborators;
    }
}

