using MediatR;

namespace AeInfinity.Application.Features.ListItems.Commands.CreateListItem;

public class CreateListItemCommand : IRequest<Guid>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1.0m;
    public string? Unit { get; set; }
    public Guid CategoryId { get; set; }
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
}

