using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Queries.GetUserInvitations;

public class GetUserInvitationsQueryHandler : IRequestHandler<GetUserInvitationsQuery, List<InvitationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserInvitationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<InvitationDto>> Handle(GetUserInvitationsQuery request, CancellationToken cancellationToken)
    {
        var invitations = await _context.UserToLists
            .Include(utl => utl.List)
            .Include(utl => utl.User)
            .Include(utl => utl.Role)
            .Include(utl => utl.InvitedByUser)
            .AsNoTracking()
            .Where(utl => utl.UserId == request.UserId && utl.IsPending)
            .OrderByDescending(utl => utl.InvitedAt)
            .Select(utl => new InvitationDto
            {
                Id = utl.Id,
                ListId = utl.ListId,
                ListName = utl.List.Name,
                UserId = utl.UserId,
                UserEmail = utl.User.Email,
                UserDisplayName = utl.User.DisplayName,
                RoleId = utl.RoleId,
                RoleName = utl.Role.Name,
                InvitedBy = utl.InvitedBy,
                InvitedByDisplayName = utl.InvitedByUser.DisplayName,
                InvitedAt = utl.InvitedAt,
                AcceptedAt = utl.AcceptedAt,
                IsPending = utl.IsPending
            })
            .ToListAsync(cancellationToken);

        return invitations;
    }
}

