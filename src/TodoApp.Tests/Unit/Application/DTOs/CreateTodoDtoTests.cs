using TodoApp.Application.DTOs;

namespace TodoApp.Tests.Unit.Application.DTOs;

public class CreateTodoDtoTests
{
    [Fact]
    public void CreateTodoDto_WithAllProperties_ShouldCreateCorrectly()
    {
        // Arrange
        var title = "Test Title";
        var description = "Test Description";
        var dueDate = DateTime.UtcNow.AddDays(1);
        var status = TodoStatus.InProgress;

        // Act
        var dto = new CreateTodoDto(title, description, dueDate, status);

        // Assert
        dto.Title.Should().Be(title);
        dto.Description.Should().Be(description);
        dto.DueDate.Should().Be(dueDate);
        dto.Status.Should().Be(status);
    }

    [Fact]
    public void CreateTodoDto_WithMinimalProperties_ShouldUseDefaults()
    {
        // Arrange
        var title = "Test Title";

        // Act
        var dto = new CreateTodoDto(title, null, null);

        // Assert
        dto.Title.Should().Be(title);
        dto.Description.Should().BeNull();
        dto.DueDate.Should().BeNull();
        dto.Status.Should().Be(TodoStatus.Todo);
    }
}
