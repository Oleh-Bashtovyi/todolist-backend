using TodoApp.Application.Features.Commands.UpdateTodoItemStatus;

namespace TodoApp.Tests.Unit.Application.Commands.UpdateTodoItemStatus;

public class UpdateTodoItemStatusCommandTests
{
    [Fact]
    public void Constructor_ShouldCreateCommand_WithValidParameters()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = TodoStatus.InProgress;

        // Act
        var command = new UpdateTodoItemStatusCommand(id, status);

        // Assert
        command.Id.Should().Be(id);
        command.Status.Should().Be(status);
    }

    [Fact]
    public void Command_ShouldBeRecord_WithEqualityComparison()
    {
        // Arrange
        var id = Guid.NewGuid();
        var status = TodoStatus.Done;
        var command1 = new UpdateTodoItemStatusCommand(id, status);
        var command2 = new UpdateTodoItemStatusCommand(id, status);

        // Act & Assert
        command1.Should().Be(command2);
    }
}