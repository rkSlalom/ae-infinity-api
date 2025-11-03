using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Lists.Queries.GetListInvitations;

/// <summary>
/// Query to get all pending invitations for a specific list (owner only)
/// </summary>
public class GetListInvitationsQuery : IRequest<List<InvitationDto>>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; } // Current user (must be owner)
}

