using AeInfinity.Application.Common.Interfaces;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Domain.Exceptions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AeInfinity.Application.Features.Lists.Queries.GetListInvitations;

public class GetListInvitationsQueryHandler : IRequestHandler<GetListInvitationsQuery, List<InvitationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetListInvitationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<InvitationDto>> Handle(GetListInvitationsQuery request, CancellationToken cancellationToken)
    {
        // Verify list exists and user is owner
        var list = await _context.Lists
            .FirstOrDefaultAsync(l => l.Id == request.ListId, cancellationToken);

        if (list == null)
        {
            throw new NotFoundException("List", request.ListId);
        }

        if (list.OwnerId != request.UserId)
        {
            throw new ForbiddenException("Only the list owner can view pending invitations.");
        }

        var invitations = await _context.UserToLists
            .Include(utl => utl.List)
            .Include(utl => utl.User)
            .Include(utl => utl.Role)
            .Include(utl => utl.InvitedByUser)
            .AsNoTracking()
            .Where(utl => utl.ListId == request.ListId && utl.IsPending)
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

