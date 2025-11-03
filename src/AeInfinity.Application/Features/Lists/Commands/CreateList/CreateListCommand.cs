using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.CreateList;

public class CreateListCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

