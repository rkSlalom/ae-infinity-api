using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.UnarchiveList;

public class UnarchiveListCommand : IRequest<Unit>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
}

