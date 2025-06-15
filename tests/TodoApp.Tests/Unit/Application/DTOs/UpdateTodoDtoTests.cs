using TodoApp.Application.DTOs;

namespace TodoApp.Tests.Unit.Application.DTOs;

public class UpdateTodoDtoTests
{
    [Fact]
    public void UpdateTodoDto_WithAllProperties_ShouldCreateCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Updated Title";
        var description = "Updated Description";
        var dueDate = DateTime.UtcNow.AddDays(2);

        // Act
        var dto = new UpdateTodoDto(id, title, description, dueDate);

        // Assert
        dto.Id.Should().Be(id);
        dto.Title.Should().Be(title);
        dto.Description.Should().Be(description);
        dto.DueDate.Should().Be(dueDate);
    }

    [Fact]
    public void UpdateTodoDto_WithNullableProperties_ShouldCreateCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "Updated Title";

        // Act
        var dto = new UpdateTodoDto(id, title, null, null);

        // Assert
        dto.Id.Should().Be(id);
        dto.Title.Should().Be(title);
        dto.Description.Should().BeNull();
        dto.DueDate.Should().BeNull();
    }
}