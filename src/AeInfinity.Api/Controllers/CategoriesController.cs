using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.Categories.Queries.GetCategories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeInfinity.Api.Controllers;

/// <summary>
/// Controller for managing categories
/// </summary>
[Authorize]
public class CategoriesController : BaseApiController
{
    /// <summary>
    /// Get all available categories
    /// </summary>
    /// <returns>List of all categories</returns>
    /// <response code="200">Returns the list of categories</response>
    /// <response code="401">Unauthorized - valid JWT token required</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        var categories = await Mediator.Send(new GetCategoriesQuery());
        return Ok(categories);
    }
}

