namespace AeInfinity.Application.Features.ListItems.Contracts;

public class ReorderItemsRequest
{
    public List<ItemPositionDto> ItemPositions { get; set; } = new();
}

public class ItemPositionDto
{
    public Guid ItemId { get; set; }
    public int Position { get; set; }
}

