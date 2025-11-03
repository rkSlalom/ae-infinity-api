using AeInfinity.Application.Common.Models.DTOs;
using MediatR;

namespace AeInfinity.Application.Features.Categories.Queries.GetCategories;

/// <summary>
/// Query to get all available categories
/// </summary>
public class GetCategoriesQuery : IRequest<List<CategoryDto>>
{
}

