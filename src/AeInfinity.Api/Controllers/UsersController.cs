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
    /// Get current authenticated user's complete profile
    /// </summary>
    /// <remarks>
    /// Retrieves the complete profile information for the authenticated user including:
    /// - Basic info (id, email, display name)
    /// - Avatar URL
    /// - Email verification status
    /// - Account timestamps (created, last login)
    /// 
    /// **Authentication**: Requires valid JWT Bearer token
    /// </remarks>
    /// <returns>User profile with all fields</returns>
    /// <response code="200">Returns the user's complete profile</response>
    /// <response code="401">Invalid or missing JWT token</response>
    /// <response code="404">User not found in database</response>
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
    /// Get public profile information for any user
    /// </summary>
    /// <remarks>
    /// Returns limited public profile information (displayName, avatarUrl) for viewing collaborators.
    /// 
    /// **Privacy**: Does NOT expose email, statistics, or other sensitive information.
    /// 
    /// **Use Cases**:
    /// - View collaborator profiles in shared lists
    /// - Display user information in activity feeds
    /// - Show user details when clicking on usernames
    /// </remarks>
    /// <param name="id">The GUID of the user to retrieve</param>
    /// <returns>Public user profile with limited fields</returns>
    /// <response code="200">Returns the user's public profile</response>
    /// <response code="401">Invalid or missing JWT token</response>
    /// <response code="404">User not found or deleted</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserBasicDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    /// Update authenticated user's profile (display name and/or avatar)
    /// </summary>
    /// <remarks>
    /// Updates the user's profile information. Only display name and avatar URL can be modified.
    /// 
    /// **Validation Rules**:
    /// - **displayName**: Required, 2-100 characters, supports Unicode/emojis
    /// - **avatarUrl**: Optional, must be valid HTTP/HTTPS URL or null to clear
    /// 
    /// **Security**: 
    /// - User ID extracted from JWT token (cannot update other users' profiles)
    /// - Users have full autonomy over their own profiles
    /// 
    /// **Real-time**: 
    /// - Broadcasts `ProfileUpdated` SignalR event to all connected clients
    /// - Updates appear immediately in Header and collaborator lists
    /// 
    /// **Example Request**:
    /// ```json
    /// {
    ///   "displayName": "Jane Smith 🎉",
    ///   "avatarUrl": "https://example.com/avatar.jpg"
    /// }
    /// ```
    /// </remarks>
    /// <param name="request">Profile update request with displayName and optional avatarUrl</param>
    /// <returns>Updated user profile</returns>
    /// <response code="200">Profile updated successfully, returns updated UserDto</response>
    /// <response code="400">Validation errors (display name length, invalid URL format)</response>
    /// <response code="401">Invalid or missing JWT token</response>
    /// <response code="404">User not found in database</response>
    [HttpPatch("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
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
        
        // Return updated user profile
        var query = new GetCurrentUserQuery { UserId = userId };
        var updatedUser = await _mediator.Send(query);
        
        return Ok(updatedUser);
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

