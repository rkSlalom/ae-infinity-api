using System.Security.Claims;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.ListItems.Commands.CreateItem;
using AeInfinity.Application.Features.ListItems.Commands.ReorderItems;
using AeInfinity.Application.Features.ListItems.Contracts;
using AeInfinity.Application.Features.ListItems.Commands.DeleteListItem;
using AeInfinity.Application.Features.ListItems.Commands.MarkItemPurchased;
using AeInfinity.Application.Features.ListItems.Commands.MarkItemUnpurchased;
using AeInfinity.Application.Features.ListItems.Commands.UpdateListItem;
using AeInfinity.Application.Features.ListItems.Queries.GetListItemById;
using AeInfinity.Application.Features.ListItems.Queries.GetListItems;
using AeInfinity.Application.Features.Search.Queries.SearchListItems;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeInfinity.Api.Controllers;

/// <summary>
/// Shopping list items management endpoints
/// </summary>
[Authorize]
public class ListItemsController : BaseApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<ListItemsController> _logger;

    public ListItemsController(IMediator mediator, ILogger<ListItemsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all items in a shopping list with optional filtering and metadata
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="categoryId">Filter by category</param>
    /// <param name="isPurchased">Filter by purchase status</param>
    /// <param name="createdBy">Filter by creator</param>
    /// <returns>Items with metadata (total count, purchased count, etc.)</returns>
    [HttpGet("/api/lists/{listId}/items")]
    [ProducesResponseType(typeof(ItemsListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemsListResponse>> GetListItems(
        Guid listId,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool? isPurchased = null,
        [FromQuery] Guid? createdBy = null)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new GetListItemsQuery
        {
            ListId = listId,
            UserId = userId,
            CategoryId = categoryId,
            IsPurchased = isPurchased,
            CreatedBy = createdBy
        };

        var response = await _mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Get a specific list item by ID
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <returns>Item details</returns>
    [HttpGet("/api/lists/{listId}/items/{itemId}")]
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
            ListId = listId,
            ItemId = itemId,
            UserId = userId
        };

        var item = await _mediator.Send(query);
        return Ok(item);
    }

    /// <summary>
    /// Create a new item in a shopping list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="request">Item creation data</param>
    /// <returns>Created item ID</returns>
    [HttpPost("/api/lists/{listId}/items")]
    [ProducesResponseType(typeof(ListItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ListItemDto>> CreateListItem(Guid listId, [FromBody] CreateItemRequest request)
    {
        _logger.LogInformation("=== CreateListItem START ===");
        _logger.LogInformation("ListId: {ListId}", listId);
        _logger.LogInformation("Request: {@Request}", request);
        
        var userId = GetCurrentUserId();
        _logger.LogInformation("UserId from token: {UserId}", userId);
        
        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Unauthorized - no user ID in token");
            return Unauthorized();
        }

        _logger.LogInformation("Creating command...");
        var command = new CreateItemCommand
        {
            ListId = listId,
            UserId = userId,
            Name = request.Name,
            Quantity = request.Quantity,
            Unit = request.Unit,
            CategoryId = request.CategoryId,
            Notes = request.Notes,
            ImageUrl = request.ImageUrl
        };
        
        _logger.LogInformation("Command created: {@Command}", command);
        _logger.LogInformation("Sending command to mediator...");

        try
        {
            var item = await _mediator.Send(command);
            _logger.LogInformation("Item created successfully with ID: {ItemId}", item.Id);
            return CreatedAtAction(nameof(GetListItemById), new { listId, itemId = item.Id }, item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating list item");
            throw;
        }
    }

    /// <summary>
    /// Update a list item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="request">Updated item data</param>
    /// <returns>No content on success</returns>
    [HttpPut("/api/lists/{listId}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateListItem(Guid listId, Guid itemId, [FromBody] UpdateListItemRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new UpdateListItemCommand
        {
            ListId = listId,
            ItemId = itemId,
            UserId = userId,
            Name = request.Name,
            Quantity = request.Quantity,
            Unit = request.Unit,
            CategoryId = request.CategoryId,
            Notes = request.Notes,
            ImageUrl = request.ImageUrl,
            Position = request.Position
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete a list item (soft delete)
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("/api/lists/{listId}/items/{itemId}")]
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
            ListId = listId,
            ItemId = itemId,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Reorder items in a list by updating their positions
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="request">List of items with their new positions</param>
    /// <returns>Success status</returns>
    [HttpPatch("/api/lists/{listId}/items/reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ReorderItems(Guid listId, [FromBody] ReorderItemsRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var command = new ReorderItemsCommand
        {
            ListId = listId,
            UserId = userId,
            Items = request.Items.Select(i => new AeInfinity.Application.Features.ListItems.Commands.ReorderItems.ItemPosition
            {
                ItemId = i.ItemId,
                Position = i.Position
            }).ToList()
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
    [HttpPatch("/api/lists/{listId}/items/{itemId}/purchase")]
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
            ListId = listId,
            ItemId = itemId,
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
    [HttpPatch("/api/lists/{listId}/items/{itemId}/unpurchase")]
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
            ListId = listId,
            ItemId = itemId,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Search items within a specific list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="q">Search query (minimum 2 characters)</param>
    /// <returns>List of matching items</returns>
    [HttpGet("/api/lists/{listId}/items/search")]
    [ProducesResponseType(typeof(List<ItemSearchResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ItemSearchResultDto>>> SearchListItems(
        Guid listId,
        [FromQuery] string q)
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

        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized();

        var query = new SearchListItemsQuery
        {
            UserId = userId,
            ListId = listId,
            Query = q
        };

        var items = await _mediator.Send(query);
        return Ok(items);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

/// <summary>
/// Request model for creating a list item
/// </summary>
public class CreateListItemRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1.0m;
    public string? Unit { get; set; }
    public Guid CategoryId { get; set; }
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
}

/// <summary>
/// Request model for updating a list item
/// </summary>
public class UpdateListItemRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1.0m;
    public string? Unit { get; set; }
    public Guid CategoryId { get; set; }
    public string? Notes { get; set; }
    public string? ImageUrl { get; set; }
    public int Position { get; set; }
}

/// <summary>
/// Request model for reordering list items
/// </summary>
public class ReorderItemsRequest
{
    public List<ItemPositionDto> Items { get; set; } = new();
}

/// <summary>
/// Item position DTO for reordering
/// </summary>
public class ItemPositionDto
{
    public Guid ItemId { get; set; }
    public int Position { get; set; }
}

