using System.Security.Claims;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.Lists.Commands.AcceptInvitation;
using AeInfinity.Application.Features.Lists.Commands.DeclineInvitation;
using AeInfinity.Application.Features.Lists.Queries.GetUserInvitations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeInfinity.Api.Controllers;

/// <summary>
/// List invitations management endpoints
/// </summary>
[Authorize]
public class InvitationsController : BaseApiController
{
    private readonly IMediator _mediator;

    public InvitationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all pending invitations for the current user
    /// </summary>
    /// <returns>List of pending invitations</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<InvitationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<InvitationDto>>> GetUserInvitations()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetUserInvitationsQuery
        {
            UserId = userId
        };

        var invitations = await _mediator.Send(query);
        return Ok(invitations);
    }

    /// <summary>
    /// Accept a list invitation
    /// </summary>
    /// <param name="id">Invitation ID</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/accept")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptInvitation(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new AcceptInvitationCommand
        {
            InvitationId = id,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Decline a list invitation
    /// </summary>
    /// <param name="id">Invitation ID</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/decline")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeclineInvitation(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new DeclineInvitationCommand
        {
            InvitationId = id,
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

