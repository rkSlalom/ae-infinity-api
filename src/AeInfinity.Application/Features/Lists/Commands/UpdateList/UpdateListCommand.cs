using MediatR;

namespace AeInfinity.Application.Features.Lists.Commands.UpdateList;

public class UpdateListCommand : IRequest<Unit>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

