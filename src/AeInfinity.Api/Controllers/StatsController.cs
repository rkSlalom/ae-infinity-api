using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.Statistics.Queries.GetUserStats;
using AeInfinity.Application.Features.Statistics.Queries.GetUserPurchaseHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeInfinity.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users/me")]
public class StatsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get authenticated user's activity statistics
    /// </summary>
    /// <remarks>
    /// Retrieves comprehensive activity statistics for the authenticated user:
    /// - **totalListsOwned**: Count of lists where user is owner
    /// - **totalListsShared**: Count of lists shared with user (as collaborator)
    /// - **totalItemsCreated**: Total items created across all lists
    /// - **totalItemsPurchased**: Total items marked as purchased
    /// - **totalActiveCollaborations**: Active (non-archived) collaborative lists
    /// - **lastActivityAt**: Timestamp of most recent activity (or null)
    /// 
    /// **Performance**: 
    /// - Cached for 5 minutes to optimize database load
    /// - Queries complete in &lt; 500ms even for users with 100+ lists
    /// - Cache invalidated when user performs relevant actions
    /// 
    /// **Use Cases**:
    /// - Display engagement metrics on profile page
    /// - Show user activity in dashboards
    /// - Track shopping list usage patterns
    /// </remarks>
    /// <returns>User activity statistics with 6 metrics</returns>
    /// <response code="200">Returns user statistics (may be cached)</response>
    /// <response code="401">Invalid or missing JWT token</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(UserStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserStatsDto>> GetUserStats()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var query = new GetUserStatsQuery
        {
            UserId = userId
        };

        var stats = await _mediator.Send(query);
        return Ok(stats);
    }

    /// <summary>
    /// Get current user's purchase history
    /// </summary>
    /// <param name="limit">Maximum number of purchases to return (default: 50, max: 200)</param>
    /// <returns>List of purchased items with details</returns>
    [HttpGet("history")]
    [ProducesResponseType(typeof(List<PurchaseHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<PurchaseHistoryDto>>> GetUserPurchaseHistory([FromQuery] int? limit = 50)
    {
        // Validate limit
        if (limit.HasValue && (limit.Value < 1 || limit.Value > 200))
        {
            return BadRequest(new
            {
                StatusCode = 400,
                Message = "Limit must be between 1 and 200.",
                Errors = new[]
                {
                    new { Property = "limit", Message = "Limit must be between 1 and 200." }
                }
            });
        }

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var query = new GetUserPurchaseHistoryQuery
        {
            UserId = userId,
            Limit = limit
        };

        var history = await _mediator.Send(query);
        return Ok(history);
    }
}

