using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.Search.Queries.SearchGlobal;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeInfinity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Search across lists and items
    /// </summary>
    /// <param name="q">Search query (minimum 2 characters)</param>
    /// <param name="scope">Search scope: "all" (default), "lists", or "items"</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20)</param>
    /// <returns>Search results with pagination</returns>
    [HttpGet]
    [ProducesResponseType(typeof(SearchResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SearchResultDto>> Search(
        [FromQuery] string q,
        [FromQuery] string scope = "all",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Validate query length
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
        {
            return BadRequest(new
            {
                StatusCode = 400,
                Message = "Search query must be at least 2 characters long.",
                Errors = new[]
                {
                    new { Property = "q", Message = "Search query must be at least 2 characters long." }
                }
            });
        }

        // Validate scope
        var validScopes = new[] { "all", "lists", "items" };
        if (!validScopes.Contains(scope.ToLower()))
        {
            return BadRequest(new
            {
                StatusCode = 400,
                Message = "Invalid scope. Must be 'all', 'lists', or 'items'.",
                Errors = new[]
                {
                    new { Property = "scope", Message = "Invalid scope. Must be 'all', 'lists', or 'items'." }
                }
            });
        }

        // Validate pagination
        if (page < 1)
        {
            return BadRequest(new
            {
                StatusCode = 400,
                Message = "Page number must be greater than 0.",
                Errors = new[]
                {
                    new { Property = "page", Message = "Page number must be greater than 0." }
                }
            });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                StatusCode = 400,
                Message = "Page size must be between 1 and 100.",
                Errors = new[]
                {
                    new { Property = "pageSize", Message = "Page size must be between 1 and 100." }
                }
            });
        }

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var query = new SearchGlobalQuery
        {
            UserId = userId,
            Query = q,
            Scope = scope.ToLower(),
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}

