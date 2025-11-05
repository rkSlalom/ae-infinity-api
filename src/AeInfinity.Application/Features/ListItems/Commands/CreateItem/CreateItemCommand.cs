using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.ListItems.Commands.CreateItem;

public class CreateItemCommand : IRequest<ListItemDto>
{
    public Guid ListId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public string? Unit { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
}

