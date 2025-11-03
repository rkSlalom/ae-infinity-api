using MediatR;

namespace AeInfinity.Application.Features.ListItems.Commands.MarkItemUnpurchased;

public class MarkItemUnpurchasedCommand : IRequest<Unit>
{
    public Guid ListId { get; set; }
    public Guid ItemId { get; set; }
    public Guid UserId { get; set; }
}

