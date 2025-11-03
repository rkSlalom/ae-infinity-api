using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.DeleteList;

public class DeleteListCommand : IRequest<Unit>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
}

