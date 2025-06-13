using TodoApp.Application.Features.Commands.DeleteTodoItem;

namespace TodoApp.Tests.Unit.Application.Commands.DeleteTodoItem;

public class DeleteTodoItemCommandTests
{
    [Fact]
    public void Constructor_ShouldCreateCommand_WithValidId()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var command = new DeleteTodoItemCommand(id);

        // Assert
        command.Id.Should().Be(id);
    }

    [Fact]
    public void Command_ShouldBeRecord_WithEqualityComparison()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command1 = new DeleteTodoItemCommand(id);
        var command2 = new DeleteTodoItemCommand(id);

        // Act & Assert
        command1.Should().Be(command2);
    }
}