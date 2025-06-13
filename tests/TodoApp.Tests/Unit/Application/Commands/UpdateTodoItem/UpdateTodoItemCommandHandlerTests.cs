using TodoApp.Application.DTOs;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Features.Commands.UpdateTodoItem;
using TodoApp.Application.Interfaces;

namespace TodoApp.Tests.Unit.Application.Commands.UpdateTodoItem;

public class UpdateTodoItemCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly UpdateTodoItemCommandHandler _handler;

    public UpdateTodoItemCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _handler = new UpdateTodoItemCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateTodoItem_WhenItemExists()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateDto = new UpdateTodoDto(itemId, "Updated Title", "Updated Description", DateTime.UtcNow.AddDays(2));
        var command = new UpdateTodoItemCommand(updateDto);

        var existingItem = new TodoItem
        {
            Id = itemId,
            Title = "Original Title",
            Description = "Original Description",
            Status = TodoStatus.Todo,
            DueDate = DateTime.UtcNow.AddDays(1),
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _repositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(itemId);
        result.Title.Should().Be(updateDto.Title);
        result.Description.Should().Be(updateDto.Description);
        result.DueDate.Should().Be(updateDto.DueDate);

        _repositoryMock.Verify(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(existingItem, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateDto = new UpdateTodoDto(itemId, "Updated Title", "Updated Description", DateTime.UtcNow.AddDays(2));
        var command = new UpdateTodoItemCommand(updateDto);

        _repositoryMock.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain("TodoItem");
        exception.Message.Should().Contain(itemId.ToString());

        _repositoryMock.Verify(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}