using TodoApp.Application.Exceptions;
using TodoApp.Application.Features.Commands.UpdateTodoItemStatus;
using TodoApp.Application.Interfaces;

namespace TodoApp.Tests.Unit.Application.Commands.UpdateTodoItemStatus;

public class UpdateTodoItemStatusCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly UpdateTodoItemStatusCommandHandler _handler;

    public UpdateTodoItemStatusCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _handler = new UpdateTodoItemStatusCommandHandler(_repositoryMock.Object);
    }

    [Theory]
    [InlineData(TodoStatus.Todo)]
    [InlineData(TodoStatus.InProgress)]
    [InlineData(TodoStatus.Done)]
    public async Task Handle_ShouldUpdateStatus_WhenItemExists(TodoStatus newStatus)
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new UpdateTodoItemStatusCommand(itemId, newStatus);

        var existingItem = new TodoItem
        {
            Id = itemId,
            Title = "Test Item",
            Status = TodoStatus.Todo,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(itemId);
        result.Status.Should().Be(newStatus);

        _repositoryMock.Verify(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new UpdateTodoItemStatusCommand(itemId, TodoStatus.Done);

        _repositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
