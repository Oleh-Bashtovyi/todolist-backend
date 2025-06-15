using TodoApp.Application.Exceptions;
using TodoApp.Application.Features.Commands.DeleteTodoItem;
using TodoApp.Application.Interfaces;

namespace TodoApp.Tests.Unit.Application.Commands.DeleteTodoItem;

public class DeleteTodoItemCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly DeleteTodoItemCommandHandler _handler;

    public DeleteTodoItemCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _handler = new DeleteTodoItemCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteTodoItem_WhenItemExists()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new DeleteTodoItemCommand(itemId);

        var existingItem = new TodoItem
        {
            Id = itemId,
            Title = "Test Item",
            Status = TodoStatus.Todo,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);
        _repositoryMock.Setup(x => x.DeleteAsync(existingItem, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        _repositoryMock.Verify(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.DeleteAsync(existingItem, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var command = new DeleteTodoItemCommand(itemId);

        _repositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.DeleteAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}