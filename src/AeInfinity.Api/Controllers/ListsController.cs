using System.Security.Claims;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.Lists.Commands.ArchiveList;
using AeInfinity.Application.Features.Lists.Commands.CreateList;
using AeInfinity.Application.Features.Lists.Commands.DeleteList;
using AeInfinity.Application.Features.Lists.Commands.UnarchiveList;
using AeInfinity.Application.Features.Lists.Commands.UpdateList;
using AeInfinity.Application.Features.Lists.Queries.GetListById;
using AeInfinity.Application.Features.Lists.Queries.GetLists;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeInfinity.Api.Controllers;

/// <summary>
/// Shopping lists management endpoints
/// </summary>
[Authorize]
public class ListsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ListsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all lists for the current user (owned and shared)
    /// </summary>
    /// <param name="includeArchived">Include archived lists in the response</param>
    /// <returns>List of shopping lists</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ListDto>>> GetLists([FromQuery] bool includeArchived = false)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetListsQuery
        {
            UserId = userId,
            IncludeArchived = includeArchived
        };

        var lists = await _mediator.Send(query);
        return Ok(lists);
    }

    /// <summary>
    /// Get a specific list by ID with full details
    /// </summary>
    /// <param name="id">List ID</param>
    /// <returns>List details including items and collaborators</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ListDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListDetailDto>> GetListById(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetListByIdQuery
        {
            ListId = id,
            UserId = userId
        };

        var list = await _mediator.Send(query);
        return Ok(list);
    }

    /// <summary>
    /// Create a new shopping list
    /// </summary>
    /// <param name="request">List creation data</param>
    /// <returns>Created list ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Guid>> CreateList([FromBody] CreateListRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new CreateListCommand
        {
            UserId = userId,
            Name = request.Name,
            Description = request.Description
        };

        var listId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetListById), new { id = listId }, listId);
    }

    /// <summary>
    /// Update list details (name and description)
    /// </summary>
    /// <param name="id">List ID</param>
    /// <param name="request">Updated list data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateList(Guid id, [FromBody] UpdateListRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new UpdateListCommand
        {
            ListId = id,
            UserId = userId,
            Name = request.Name,
            Description = request.Description
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete a list (soft delete, owner only)
    /// </summary>
    /// <param name="id">List ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteList(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new DeleteListCommand
        {
            ListId = id,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Archive a list (owner only)
    /// </summary>
    /// <param name="id">List ID</param>
    /// <returns>No content on success</returns>
    [HttpPatch("{id}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ArchiveList(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new ArchiveListCommand
        {
            ListId = id,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Unarchive a list (owner only)
    /// </summary>
    /// <param name="id">List ID</param>
    /// <returns>No content on success</returns>
    [HttpPatch("{id}/unarchive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnarchiveList(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new UnarchiveListCommand
        {
            ListId = id,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

/// <summary>
/// Request model for creating a list
/// </summary>
public class CreateListRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// Request model for updating a list
/// </summary>
public class UpdateListRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

