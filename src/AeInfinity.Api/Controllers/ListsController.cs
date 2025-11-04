using System.Security.Claims;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.Lists.Commands.ArchiveList;
using AeInfinity.Application.Features.Lists.Commands.ChangeCollaboratorRole;
using AeInfinity.Application.Features.Lists.Commands.CreateList;
using AeInfinity.Application.Features.Lists.Commands.DeleteList;
using AeInfinity.Application.Features.Lists.Commands.LeaveList;
using AeInfinity.Application.Features.Lists.Commands.RemoveCollaborator;
using AeInfinity.Application.Features.Lists.Commands.ShareList;
using AeInfinity.Application.Features.Lists.Commands.UnarchiveList;
using AeInfinity.Application.Features.Lists.Commands.UpdateList;
using AeInfinity.Application.Features.Lists.Queries.GetListById;
using AeInfinity.Application.Features.Lists.Queries.GetListCollaborators;
using AeInfinity.Application.Features.Lists.Queries.GetListInvitations;
using AeInfinity.Application.Features.Lists.Queries.GetLists;
using AeInfinity.Application.Features.Statistics.Queries.GetListStats;
using AeInfinity.Application.Features.Statistics.Queries.GetListPurchaseHistory;
using AeInfinity.Application.Features.Statistics.Queries.GetListActivity;
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

    /// <summary>
    /// Share a list with another user (send invitation)
    /// </summary>
    /// <param name="id">List ID</param>
    /// <param name="request">Share request with invitee email and role</param>
    /// <returns>Invitation ID</returns>
    [HttpPost("{id}/share")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> ShareList(Guid id, [FromBody] ShareListRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new ShareListCommand
        {
            ListId = id,
            UserId = userId,
            InviteeEmail = request.InviteeEmail,
            RoleId = request.RoleId
        };

        var invitationId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetListInvitations), new { id }, invitationId);
    }

    /// <summary>
    /// Get all collaborators for a list
    /// </summary>
    /// <param name="id">List ID</param>
    /// <returns>List of collaborators</returns>
    [HttpGet("{id}/collaborators")]
    [ProducesResponseType(typeof(List<CollaboratorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<CollaboratorDto>>> GetListCollaborators(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetListCollaboratorsQuery
        {
            ListId = id,
            UserId = userId
        };

        var collaborators = await _mediator.Send(query);
        return Ok(collaborators);
    }

    /// <summary>
    /// Get all pending invitations for a list (owner only)
    /// </summary>
    /// <param name="id">List ID</param>
    /// <returns>List of pending invitations</returns>
    [HttpGet("{id}/invitations")]
    [ProducesResponseType(typeof(List<InvitationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<InvitationDto>>> GetListInvitations(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetListInvitationsQuery
        {
            ListId = id,
            UserId = userId
        };

        var invitations = await _mediator.Send(query);
        return Ok(invitations);
    }

    /// <summary>
    /// Remove a collaborator from a list (owner only)
    /// </summary>
    /// <param name="id">List ID</param>
    /// <param name="collaboratorUserId">User ID of collaborator to remove</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}/collaborators/{collaboratorUserId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveCollaborator(Guid id, Guid collaboratorUserId)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new RemoveCollaboratorCommand
        {
            ListId = id,
            CollaboratorUserId = collaboratorUserId,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Change a collaborator's role (owner only)
    /// </summary>
    /// <param name="id">List ID</param>
    /// <param name="collaboratorUserId">User ID of collaborator</param>
    /// <param name="request">New role information</param>
    /// <returns>No content on success</returns>
    [HttpPatch("{id}/collaborators/{collaboratorUserId}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeCollaboratorRole(Guid id, Guid collaboratorUserId, [FromBody] ChangeRoleRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new ChangeCollaboratorRoleCommand
        {
            ListId = id,
            CollaboratorUserId = collaboratorUserId,
            NewRoleId = request.NewRoleId,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Leave a list (collaborators only, not owner)
    /// </summary>
    /// <param name="id">List ID</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/leave")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LeaveList(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new LeaveListCommand
        {
            ListId = id,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Get statistics for a specific list
    /// </summary>
    /// <param name="id">List ID</param>
    /// <returns>List statistics including item counts and collaborator count</returns>
    [HttpGet("{id}/stats")]
    [ProducesResponseType(typeof(ListStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListStatsDto>> GetListStats(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetListStatsQuery
        {
            UserId = userId,
            ListId = id
        };

        var stats = await _mediator.Send(query);
        return Ok(stats);
    }

    /// <summary>
    /// Get purchase history for a specific list
    /// </summary>
    /// <param name="id">List ID</param>
    /// <param name="limit">Maximum number of purchases to return (default: 50, max: 200)</param>
    /// <returns>List of purchased items with details</returns>
    [HttpGet("{id}/history")]
    [ProducesResponseType(typeof(List<PurchaseHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PurchaseHistoryDto>>> GetListPurchaseHistory(Guid id, [FromQuery] int? limit = 50)
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

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetListPurchaseHistoryQuery
        {
            UserId = userId,
            ListId = id,
            Limit = limit
        };

        var history = await _mediator.Send(query);
        return Ok(history);
    }

    /// <summary>
    /// Get activity log for a specific list
    /// </summary>
    /// <param name="id">List ID</param>
    /// <param name="limit">Maximum number of activities to return (default: 20, max: 100)</param>
    /// <returns>List of recent activities</returns>
    [HttpGet("{id}/activity")]
    [ProducesResponseType(typeof(List<ListActivityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ListActivityDto>>> GetListActivity(Guid id, [FromQuery] int? limit = 20)
    {
        // Validate limit
        if (limit.HasValue && (limit.Value < 1 || limit.Value > 100))
        {
            return BadRequest(new
            {
                StatusCode = 400,
                Message = "Limit must be between 1 and 100.",
                Errors = new[]
                {
                    new { Property = "limit", Message = "Limit must be between 1 and 100." }
                }
            });
        }

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetListActivityQuery
        {
            UserId = userId,
            ListId = id,
            Limit = limit
        };

        var activity = await _mediator.Send(query);
        return Ok(activity);
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

/// <summary>
/// Request model for sharing a list
/// </summary>
public class ShareListRequest
{
    public string InviteeEmail { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
}

/// <summary>
/// Request model for changing a collaborator's role
/// </summary>
public class ChangeRoleRequest
{
    public Guid NewRoleId { get; set; }
}

