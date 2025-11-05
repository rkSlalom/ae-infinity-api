using AeInfinity.Api.Controllers;
using AeInfinity.Application.Common.Models.DTOs;
using AeInfinity.Application.Features.ListItems.Commands.CreateListItem;
using AeInfinity.Application.Features.ListItems.Commands.DeleteListItem;
using AeInfinity.Application.Features.ListItems.Commands.MarkItemPurchased;
using AeInfinity.Application.Features.ListItems.Commands.MarkItemUnpurchased;
using AeInfinity.Application.Features.ListItems.Commands.UpdateListItem;
using AeInfinity.Application.Features.ListItems.Queries.GetListItemById;
using AeInfinity.Application.Features.ListItems.Queries.GetListItems;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace AeInfinity.Api.Tests.Controllers;

public class ItemsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ItemsController _controller;
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly Guid _testListId = Guid.NewGuid();
    private readonly Guid _testItemId = Guid.NewGuid();

    public ItemsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ItemsController(_mediatorMock.Object);

        // Setup HttpContext with authenticated user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetListItems_ReturnsOkResult_WithListOfItems()
    {
        // Arrange
        var items = new List<ListItemDto>
        {
            new ListItemDto { Id = Guid.NewGuid(), Name = "Test Item 1" },
            new ListItemDto { Id = Guid.NewGuid(), Name = "Test Item 2" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetListItemsQuery>(), default))
            .ReturnsAsync(items);

        // Act
        var result = await _controller.GetListItems(_testListId, true);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedItems = Assert.IsType<List<ListItemDto>>(okResult.Value);
        Assert.Equal(2, returnedItems.Count);
    }

    [Fact]
    public async Task GetListItems_WithIncludeCompletedFalse_PassesParameterCorrectly()
    {
        // Arrange
        GetListItemsQuery? capturedQuery = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetListItemsQuery>(), default))
            .Callback<GetListItemsQuery, CancellationToken>((query, _) => capturedQuery = query)
            .ReturnsAsync(new List<ListItemDto>());

        // Act
        await _controller.GetListItems(_testListId, false);

        // Assert
        Assert.NotNull(capturedQuery);
        Assert.False(capturedQuery!.IncludeCompleted);
        Assert.Equal(_testListId, capturedQuery.ListId);
        Assert.Equal(_testUserId, capturedQuery.UserId);
    }

    [Fact]
    public async Task GetListItemById_ReturnsOkResult_WithItem()
    {
        // Arrange
        var item = new ListItemDto { Id = _testItemId, Name = "Test Item" };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetListItemByIdQuery>(), default))
            .ReturnsAsync(item);

        // Act
        var result = await _controller.GetListItemById(_testListId, _testItemId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedItem = Assert.IsType<ListItemDto>(okResult.Value);
        Assert.Equal(_testItemId, returnedItem.Id);
    }

    [Fact]
    public async Task CreateListItem_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var request = new CreateItemRequest
        {
            Name = "New Item",
            Quantity = 2,
            Unit = "lbs"
        };

        var createdItemId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateListItemCommand>(), default))
            .ReturnsAsync(createdItemId);

        // Act
        var result = await _controller.CreateListItem(_testListId, request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(createdItemId, createdResult.Value);
        Assert.Equal(nameof(ItemsController.GetListItemById), createdResult.ActionName);
    }

    [Fact]
    public async Task CreateListItem_PassesCorrectDataToCommand()
    {
        // Arrange
        var request = new CreateItemRequest
        {
            Name = "Test Item",
            Quantity = 5,
            Unit = "kg",
            Notes = "Test notes",
            CategoryId = Guid.NewGuid()
        };

        CreateListItemCommand? capturedCommand = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateListItemCommand>(), default))
            .Callback<CreateListItemCommand, CancellationToken>((cmd, _) => capturedCommand = cmd)
            .ReturnsAsync(Guid.NewGuid());

        // Act
        await _controller.CreateListItem(_testListId, request);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(request.Name, capturedCommand!.Name);
        Assert.Equal(request.Quantity, capturedCommand.Quantity);
        Assert.Equal(request.Unit, capturedCommand.Unit);
        Assert.Equal(request.Notes, capturedCommand.Notes);
        Assert.Equal(request.CategoryId, capturedCommand.CategoryId);
        Assert.Equal(_testListId, capturedCommand.ListId);
        Assert.Equal(_testUserId, capturedCommand.UserId);
    }

    [Fact]
    public async Task UpdateListItem_ReturnsNoContentResult()
    {
        // Arrange
        var request = new UpdateItemRequest
        {
            Name = "Updated Item",
            Quantity = 3
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateListItemCommand>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateListItem(_testListId, _testItemId, request);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteListItem_ReturnsNoContentResult()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteListItemCommand>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteListItem(_testListId, _testItemId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task MarkItemPurchased_ReturnsNoContentResult()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<MarkItemPurchasedCommand>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.MarkItemPurchased(_testListId, _testItemId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task MarkItemUnpurchased_ReturnsNoContentResult()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<MarkItemUnpurchasedCommand>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.MarkItemUnpurchased(_testListId, _testItemId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task MarkItemPurchased_PassesCorrectDataToCommand()
    {
        // Arrange
        MarkItemPurchasedCommand? capturedCommand = null;
        _mediatorMock.Setup(m => m.Send(It.IsAny<MarkItemPurchasedCommand>(), default))
            .Callback<MarkItemPurchasedCommand, CancellationToken>((cmd, _) => capturedCommand = cmd)
            .Returns(Task.CompletedTask);

        // Act
        await _controller.MarkItemPurchased(_testListId, _testItemId);

        // Assert
        Assert.NotNull(capturedCommand);
        Assert.Equal(_testItemId, capturedCommand!.ItemId);
        Assert.Equal(_testListId, capturedCommand.ListId);
        Assert.Equal(_testUserId, capturedCommand.UserId);
    }

    [Fact]
    public async Task GetListItems_WithUnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var controller = new ItemsController(_mediatorMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        var result = await controller.GetListItems(_testListId, true);

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task CreateListItem_WithUnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var controller = new ItemsController(_mediatorMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var request = new CreateItemRequest { Name = "Test" };

        // Act
        var result = await controller.CreateListItem(_testListId, request);

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }
}

