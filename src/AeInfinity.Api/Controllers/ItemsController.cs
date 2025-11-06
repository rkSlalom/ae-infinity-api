using System.Security.Claims;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.ListItems.Commands.CreateListItem;
using AeInfinity.Application.Features.ListItems.Commands.DeleteListItem;
using AeInfinity.Application.Features.ListItems.Commands.MarkItemPurchased;
using AeInfinity.Application.Features.ListItems.Commands.MarkItemUnpurchased;
using AeInfinity.Application.Features.ListItems.Commands.UpdateListItem;
using AeInfinity.Application.Features.ListItems.Queries.GetListItemById;
using AeInfinity.Application.Features.ListItems.Queries.GetListItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeInfinity.Api.Controllers;

/// <summary>
/// List items management endpoints
/// </summary>
[Authorize]
public class ItemsController : BaseApiController
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all items for a specific list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="includeCompleted">Include purchased items in response (default: true)</param>
    /// <returns>List of items</returns>
    [HttpGet("lists/{listId}/items")]
    [ProducesResponseType(typeof(List<ListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ListItemDto>>> GetListItems(
        Guid listId, 
        [FromQuery] bool includeCompleted = true)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetListItemsQuery
        {
            ListId = listId,
            UserId = userId,
            IncludeCompleted = includeCompleted
        };

        var items = await _mediator.Send(query);
        return Ok(items);
    }

    /// <summary>
    /// Get a specific item by ID
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <returns>Item details</returns>
    [HttpGet("lists/{listId}/items/{itemId}")]
    [ProducesResponseType(typeof(ListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ListItemDto>> GetListItemById(Guid listId, Guid itemId)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetListItemByIdQuery
        {
            ItemId = itemId,
            ListId = listId,
            UserId = userId
        };

        var item = await _mediator.Send(query);
        return Ok(item);
    }

    /// <summary>
    /// Add a new item to a list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="request">Item creation data</param>
    /// <returns>Created item ID</returns>
    [HttpPost("lists/{listId}/items")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateListItem(Guid listId, [FromBody] CreateItemRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new CreateListItemCommand
        {
            ListId = listId,
            UserId = userId,
            Name = request.Name,
            Quantity = request.Quantity,
            Unit = request.Unit,
            Notes = request.Notes,
            CategoryId = request.CategoryId
        };

        var itemId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetListItemById), new { listId, itemId }, itemId);
    }

    /// <summary>
    /// Update an existing item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="request">Updated item data</param>
    /// <returns>No content on success</returns>
    [HttpPut("lists/{listId}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateListItem(Guid listId, Guid itemId, [FromBody] UpdateItemRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new UpdateListItemCommand
        {
            ItemId = itemId,
            ListId = listId,
            UserId = userId,
            Name = request.Name,
            Quantity = request.Quantity,
            Unit = request.Unit,
            Notes = request.Notes,
            CategoryId = request.CategoryId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete an item from a list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("lists/{listId}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteListItem(Guid listId, Guid itemId)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new DeleteListItemCommand
        {
            ItemId = itemId,
            ListId = listId,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Mark an item as purchased
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <returns>No content on success</returns>
    [HttpPatch("lists/{listId}/items/{itemId}/purchase")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkItemPurchased(Guid listId, Guid itemId)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new MarkItemPurchasedCommand
        {
            ItemId = itemId,
            ListId = listId,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Mark an item as unpurchased
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <returns>No content on success</returns>
    [HttpPatch("lists/{listId}/items/{itemId}/unpurchase")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkItemUnpurchased(Guid listId, Guid itemId)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new MarkItemUnpurchasedCommand
        {
            ItemId = itemId,
            ListId = listId,
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
/// Request model for creating an item
/// </summary>
public class CreateItemRequest
{
    public string Name { get; set; } = string.Empty;
    public int? Quantity { get; set; }
    public string? Unit { get; set; }
    public string? Notes { get; set; }
    public Guid? CategoryId { get; set; }
}

/// <summary>
/// Request model for updating an item
/// </summary>
public class UpdateItemRequest
{
    public string Name { get; set; } = string.Empty;
    public int? Quantity { get; set; }
    public string? Unit { get; set; }
    public string? Notes { get; set; }
    public Guid? CategoryId { get; set; }
}

