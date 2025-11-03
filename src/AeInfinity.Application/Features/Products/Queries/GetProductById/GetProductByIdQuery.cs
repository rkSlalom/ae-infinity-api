using AeInfinity.Application.Features.Products.Queries.GetProducts;
using MediatR;

namespace AeInfinity.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;

