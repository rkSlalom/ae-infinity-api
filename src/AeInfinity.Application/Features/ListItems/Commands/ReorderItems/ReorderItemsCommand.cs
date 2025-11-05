using MediatR;

namespace AeInfinity.Application.Features.ListItems.Commands.ReorderItems;

public class ReorderItemsCommand : IRequest<Unit>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
    public List<ItemPosition> Items { get; set; } = new();
}

public class ItemPosition
{
    public Guid ItemId { get; set; }
    public int Position { get; set; }
}

