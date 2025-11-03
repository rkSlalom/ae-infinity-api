using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.ShareList;

/// <summary>
/// Command to share a list with another user (send invitation)
/// </summary>
public class ShareListCommand : IRequest<Guid>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; } // Current user (must be owner)
    public string InviteeEmail { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}

