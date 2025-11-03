using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.LeaveList;

/// <summary>
/// Command for a collaborator to leave a list (cannot be owner)
/// </summary>
public class LeaveListCommand : IRequest<Unit>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; } // Current user leaving
}

