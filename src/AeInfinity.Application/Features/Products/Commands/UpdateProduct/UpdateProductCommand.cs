using MediatR;

namespace AeInfinity.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    bool IsActive
) : IRequest<Unit>;

