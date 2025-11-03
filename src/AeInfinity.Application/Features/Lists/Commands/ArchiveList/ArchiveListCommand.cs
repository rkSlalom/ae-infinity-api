using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.ArchiveList;

public class ArchiveListCommand : IRequest<Unit>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
}

