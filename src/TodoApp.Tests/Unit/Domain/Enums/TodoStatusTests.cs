using System.ComponentModel;

namespace TodoApp.Tests.Unit.Domain.Enums;

public class TodoStatusTests
{
    [Theory]
    [InlineData(TodoStatus.Todo, 0)]
    [InlineData(TodoStatus.InProgress, 1)]
    [InlineData(TodoStatus.Done, 2)]
    public void TodoStatus_ShouldHaveCorrectValues(TodoStatus status, int expectedValue)
    {
        // Act & Assert
        ((int)status).Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(TodoStatus.Todo, "Pending task")]
    [InlineData(TodoStatus.InProgress, "Task in progress")]
    [InlineData(TodoStatus.Done, "Completed task")]
    public void TodoStatus_ShouldHaveCorrectDescriptions(TodoStatus status, string expectedDescription)
    {
        // Arrange
        var field = status.GetType().GetField(status.ToString());
        var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute));

        // Act & Assert
        attribute.Should().NotBeNull();
        attribute!.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void TodoStatus_ShouldHaveThreeValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<TodoStatus>();

        // Assert
        values.Should().HaveCount(3);
        values.Should().Contain([TodoStatus.Todo, TodoStatus.InProgress, TodoStatus.Done]);
    }
}