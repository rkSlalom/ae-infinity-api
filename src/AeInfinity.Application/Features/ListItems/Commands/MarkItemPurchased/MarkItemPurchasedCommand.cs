using MediatR;

namespace AeInfinity.Application.Features.ListItems.Commands.MarkItemPurchased;

public class MarkItemPurchasedCommand : IRequest<Unit>
{
    public Guid ListId { get; set; }
    public Guid ItemId { get; set; }
    public Guid UserId { get; set; }
}

