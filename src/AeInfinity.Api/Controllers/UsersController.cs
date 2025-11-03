using System.Security.Claims;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.Users.Commands.DeleteUserAccount;
using AeInfinity.Application.Features.Users.Commands.UpdateUserProfile;
using AeInfinity.Application.Features.Users.Queries.GetCurrentUser;
using AeInfinity.Application.Features.Users.Queries.GetUserById;
using AeInfinity.Application.Features.Users.Queries.SearchUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeInfinity.Api.Controllers;

/// <summary>
/// User management endpoints
/// </summary>
[Authorize]
public class UsersController : BaseApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    /// <returns>Current user details</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetCurrentUserQuery { UserId = userId };
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get user by ID (public profile information)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User basic information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserBasicDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserBasicDto>> GetUserById(Guid id)
    {
        var query = new GetUserByIdQuery { UserId = id };
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Search users by email or display name
    /// </summary>
    /// <param name="q">Search query (email or display name)</param>
    /// <returns>List of matching users</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<UserBasicDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserBasicDto>>> SearchUsers([FromQuery] string q)
    {
        var query = new SearchUsersQuery { Query = q ?? string.Empty };
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Update own user profile
    /// </summary>
    /// <param name="request">Profile update request</param>
    /// <returns>No content on success</returns>
    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile([FromBody] Application.Common.Models.DTOs.UpdateUserProfileRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var command = new UpdateUserProfileCommand
        {
            UserId = userId,
            DisplayName = request.DisplayName,
            AvatarUrl = request.AvatarUrl
        };

        await _mediator.Send(command);
        
        return NoContent();
    }

    /// <summary>
    /// Delete own user account (soft delete)
    /// </summary>
    /// <returns>No content on success</returns>
    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var command = new DeleteUserAccountCommand { UserId = userId };
        await _mediator.Send(command);
        
        return NoContent();
    }
}

