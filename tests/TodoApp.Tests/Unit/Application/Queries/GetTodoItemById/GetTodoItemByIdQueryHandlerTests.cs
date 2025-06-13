using TodoApp.Application.Exceptions;
using TodoApp.Application.Features.Queries.GetTodoItemById;
using TodoApp.Application.Interfaces;

namespace TodoApp.Tests.Unit.Application.Queries.GetTodoItemById;

public class GetTodoItemByIdQueryHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly GetTodoItemByIdQueryHandler _handler;

    public GetTodoItemByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _handler = new GetTodoItemByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTodoItem_WhenItemExists()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var query = new GetTodoItemByIdQuery(itemId);

        var todoItem = new TodoItem
        {
            Id = itemId,
            Title = "Test Item",
            Description = "Test Description",
            Status = TodoStatus.Todo,
            DueDate = DateTime.UtcNow.AddDays(1),
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoItem);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(itemId);
        result.Title.Should().Be("Test Item");
        result.Description.Should().Be("Test Description");
        result.Status.Should().Be(TodoStatus.Todo);

        _repositoryMock.Verify(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var query = new GetTodoItemByIdQuery(itemId);

        _repositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(query, CancellationToken.None));

        exception.Message.Should().Contain("TodoItem");
        exception.Message.Should().Contain(itemId.ToString());

        _repositoryMock.Verify(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
    }
}