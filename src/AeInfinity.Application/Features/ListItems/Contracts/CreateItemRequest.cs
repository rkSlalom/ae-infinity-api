namespace AeInfinity.Application.Features.ListItems.Contracts;

public class CreateItemRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public string? Unit { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
}

