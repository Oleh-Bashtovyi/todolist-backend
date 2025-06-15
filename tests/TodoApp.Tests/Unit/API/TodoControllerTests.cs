using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoApp.API.Controllers;
using TodoApp.API.Models.Requests;
using TodoApp.Application.DTOs;
using TodoApp.Application.Features.Commands.CreateTodoItem;
using TodoApp.Application.Features.Commands.DeleteTodoItem;
using TodoApp.Application.Features.Commands.UpdateTodoItem;
using TodoApp.Application.Features.Commands.UpdateTodoItemStatus;
using TodoApp.Application.Features.Queries.GetAllTodoItems;
using TodoApp.Application.Features.Queries.GetTodoItemById;

namespace TodoApp.Tests.Unit.API;

public class TodoControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<TodosController>> _loggerMock;
    private readonly TodosController _controller;

    public TodoControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<TodosController>>();
        _controller = new TodosController(_mediatorMock.Object, _loggerMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ShouldReturnOkWithTodos()
    {
        // Arrange
        var todos = CreateTodoListFixture();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllTodoItemsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(todos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(todos);

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllTodoItemsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_WithCancellationToken_ShouldPassTokenToMediator()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var todos = CreateTodoListFixture();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllTodoItemsQuery>(), cancellationToken))
            .ReturnsAsync(todos);

        // Act
        await _controller.GetAll(cancellationToken);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<GetAllTodoItemsQuery>(), cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoTodosExist()
    {
        // Arrange
        var emptyTodos = new List<TodoDto>();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllTodoItemsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyTodos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(emptyTodos);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WithExistingId_ShouldReturnOkWithTodo()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var todo = CreateTodoFixture(todoId);
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTodoItemByIdQuery>(q => q.Id == todoId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(todo);

        // Act
        var result = await _controller.GetById(todoId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(todo);
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTodoItemByIdQuery>(q => q.Id == todoId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoDto?)null);

        // Act
        var result = await _controller.GetById(todoId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult!.Value.Should().Be($"Todo with id {todoId} not found");
    }

    [Fact]
    public async Task GetById_WithCancellationToken_ShouldPassTokenToMediator()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var todo = CreateTodoFixture(todoId);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTodoItemByIdQuery>(q => q.Id == todoId), cancellationToken))
            .ReturnsAsync(todo);

        // Act
        await _controller.GetById(todoId, cancellationToken);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.Is<GetTodoItemByIdQuery>(q => q.Id == todoId), cancellationToken),
            Times.Once);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidDto_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateTodoDto("Test Todo", "Description", DateTime.UtcNow.AddDays(1));
        var createdTodo = CreateTodoFixture();

        _mediatorMock
            .Setup(m => m.Send(It.Is<CreateTodoItemCommand>(c => c.TodoDto == createDto), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTodo);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtResult = result.Result as CreatedAtActionResult;
        createdAtResult!.ActionName.Should().Be(nameof(TodosController.GetById));
        createdAtResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(createdTodo.Id);
        createdAtResult.Value.Should().BeEquivalentTo(createdTodo);
    }

    [Fact]
    public async Task Create_WithCancellationToken_ShouldPassTokenToMediator()
    {
        // Arrange
        var createDto = new CreateTodoDto("Test Todo", "Description", DateTime.UtcNow.AddDays(1));
        var createdTodo = CreateTodoFixture();
        var cancellationToken = CancellationToken.None;

        _mediatorMock
            .Setup(m => m.Send(It.Is<CreateTodoItemCommand>(c => c.TodoDto == createDto), cancellationToken))
            .ReturnsAsync(createdTodo);

        // Act
        await _controller.Create(createDto, cancellationToken);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.Is<CreateTodoItemCommand>(c => c.TodoDto == createDto), cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Create_WithMinimalDto_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateTodoDto("Test Todo", null, null);
        var createdTodo = CreateTodoFixture(title: "Test Todo");

        _mediatorMock
            .Setup(m => m.Send(It.Is<CreateTodoItemCommand>(c => c.TodoDto == createDto), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTodo);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtResult = result.Result as CreatedAtActionResult;
        createdAtResult!.Value.Should().BeEquivalentTo(createdTodo);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WithValidDto_ShouldReturnOkWithUpdatedTodo()
    {
        // Arrange
        var updateDto = new UpdateTodoDto(Guid.NewGuid(), "Updated Title", "Updated Description", DateTime.UtcNow.AddDays(2));
        var updatedTodo = CreateTodoFixture(updateDto.Id, updateDto.Title);

        _mediatorMock
            .Setup(m => m.Send(It.Is<UpdateTodoItemCommand>(c => c.TodoDto == updateDto), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTodo);

        // Act
        var result = await _controller.Update(updateDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(updatedTodo);
    }

    [Fact]
    public async Task Update_WithCancellationToken_ShouldPassTokenToMediator()
    {
        // Arrange
        var updateDto = new UpdateTodoDto(Guid.NewGuid(), "Updated Title", "Updated Description", DateTime.UtcNow.AddDays(2));
        var updatedTodo = CreateTodoFixture(updateDto.Id, updateDto.Title);
        var cancellationToken = new CancellationToken();

        _mediatorMock
            .Setup(m => m.Send(It.Is<UpdateTodoItemCommand>(c => c.TodoDto == updateDto), cancellationToken))
            .ReturnsAsync(updatedTodo);

        // Act
        await _controller.Update(updateDto, cancellationToken);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.Is<UpdateTodoItemCommand>(c => c.TodoDto == updateDto), cancellationToken),
            Times.Once);
    }

    #endregion

    #region UpdateStatus Tests

    [Fact]
    public async Task UpdateStatus_WithValidStatus_ShouldReturnOkWithUpdatedTodo()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var request = new UpdateTodoStatusRequest(TodoStatus.Done);
        var updatedTodo = CreateTodoFixture(todoId, status: TodoStatus.Done);

        _mediatorMock
            .Setup(m => m.Send(It.Is<UpdateTodoItemStatusCommand>(c => c.Id == todoId && c.Status == request.Status), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTodo);

        // Act
        var result = await _controller.UpdateStatus(todoId, request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(updatedTodo);
    }

    [Fact]
    public async Task UpdateStatus_WithInvalidStatus_ShouldReturnBadRequest()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var invalidStatus = (TodoStatus)999;
        var request = new UpdateTodoStatusRequest(invalidStatus);

        // Act
        var result = await _controller.UpdateStatus(todoId, request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be($"Invalid status value: {invalidStatus}");
    }

    [Theory]
    [InlineData(TodoStatus.Todo)]
    [InlineData(TodoStatus.InProgress)]
    [InlineData(TodoStatus.Done)]
    public async Task UpdateStatus_WithAllValidStatuses_ShouldReturnOk(TodoStatus status)
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var request = new UpdateTodoStatusRequest(status);
        var updatedTodo = CreateTodoFixture(todoId, status: status);

        _mediatorMock
            .Setup(m => m.Send(It.Is<UpdateTodoItemStatusCommand>(c => c.Id == todoId && c.Status == status), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTodo);

        // Act
        var result = await _controller.UpdateStatus(todoId, request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UpdateStatus_WithCancellationToken_ShouldPassTokenToMediator()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var request = new UpdateTodoStatusRequest(TodoStatus.Done);
        var updatedTodo = CreateTodoFixture(todoId, status: TodoStatus.Done);
        var cancellationToken = new CancellationToken();

        _mediatorMock
            .Setup(m => m.Send(It.Is<UpdateTodoItemStatusCommand>(c => c.Id == todoId && c.Status == request.Status), cancellationToken))
            .ReturnsAsync(updatedTodo);

        // Act
        await _controller.UpdateStatus(todoId, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.Is<UpdateTodoItemStatusCommand>(c => c.Id == todoId && c.Status == request.Status), cancellationToken),
            Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithExistingId_ShouldReturnOkWithDeletedTrue()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteTodoItemCommand>(c => c.Id == todoId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(todoId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;

        var anonymousObject = okResult!.Value;
        var deletedProperty = anonymousObject!.GetType().GetProperty("Deleted");
        var deletedValue = deletedProperty!.GetValue(anonymousObject);
        deletedValue.Should().Be(true);
    }

    [Fact]
    public async Task Delete_WithNonExistingId_ShouldReturnOkWithDeletedFalse()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteTodoItemCommand>(c => c.Id == todoId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(todoId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;

        var anonymousObject = okResult!.Value;
        var deletedProperty = anonymousObject!.GetType().GetProperty("Deleted");
        var deletedValue = deletedProperty!.GetValue(anonymousObject);
        deletedValue.Should().Be(false);
    }

    [Fact]
    public async Task Delete_WithCancellationToken_ShouldPassTokenToMediator()
    {
        // Arrange
        var todoId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteTodoItemCommand>(c => c.Id == todoId), cancellationToken))
            .ReturnsAsync(true);

        // Act
        await _controller.Delete(todoId, cancellationToken);

        // Assert
        _mediatorMock.Verify(
            m => m.Send(It.Is<DeleteTodoItemCommand>(c => c.Id == todoId), cancellationToken),
            Times.Once);
    }

    #endregion

    #region Test Fixtures

    private static TodoDto CreateTodoFixture(Guid? id = null, string title = "Test Todo", TodoStatus status = TodoStatus.Todo)
    {
        return new TodoDto(
            id ?? Guid.NewGuid(),
            title,
            "Test Description",
            status,
            DateTime.UtcNow.AddDays(1),
            true,
            false,
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    private static List<TodoDto> CreateTodoListFixture()
    {
        return new List<TodoDto>
        {
            CreateTodoFixture(Guid.NewGuid(), "Todo 1", TodoStatus.Todo),
            CreateTodoFixture(Guid.NewGuid(), "Todo 2", TodoStatus.InProgress),
            CreateTodoFixture(Guid.NewGuid(), "Todo 3", TodoStatus.Done)
        };
    }

    #endregion
}