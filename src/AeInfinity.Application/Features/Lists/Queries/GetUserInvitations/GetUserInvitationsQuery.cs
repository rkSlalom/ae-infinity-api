using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Lists.Queries.GetUserInvitations;

/// <summary>
/// Query to get all pending invitations for the current user
/// </summary>
public class GetUserInvitationsQuery : IRequest<List<InvitationDto>>
{
    public Guid UserId { get; set; }
}

