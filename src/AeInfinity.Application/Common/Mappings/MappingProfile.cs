using AutoMapper;
using AeInfinity.Application.Features.Products.Commands.CreateProduct;
using AeInfinity.Application.Features.Products.Commands.UpdateProduct;
using AeInfinity.Application.Features.Products.Queries.GetProducts;
using AeInfinity.Domain.Entities;

namespace AeInfinity.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductCommand, Product>();
        CreateMap<UpdateProductCommand, Product>();
    }
}

