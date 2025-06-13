namespace TodoApp.Tests.Unit.Domain.Entities;


public class TodoItemTests
{
    [Fact]
    public void TodoItem_DefaultConstructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var todoItem = new TodoItem();

        // Assert
        todoItem.Id.Should().NotBeEmpty();
        todoItem.Title.Should().BeEmpty();
        todoItem.Description.Should().BeNull();
        todoItem.Status.Should().Be(TodoStatus.Todo);
        todoItem.DueDate.Should().BeNull();
        todoItem.IsCompleted.Should().BeFalse();
        todoItem.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        todoItem.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void MarkAsInProgress_ShouldUpdateStatusAndTimestamp()
    {
        // Arrange
        var todoItem = new TodoItem();
        var initialUpdatedAt = todoItem.UpdatedAt;

        // Act
        todoItem.MarkAsInProgress();

        // Assert
        todoItem.Status.Should().Be(TodoStatus.InProgress);
        todoItem.UpdatedAt.Should().NotBe(initialUpdatedAt);
        todoItem.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        todoItem.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void MarkAsCompleted_ShouldUpdateStatusAndTimestamp()
    {
        // Arrange
        var todoItem = new TodoItem();
        var initialUpdatedAt = todoItem.UpdatedAt;

        // Act
        todoItem.MarkAsCompleted();

        // Assert
        todoItem.Status.Should().Be(TodoStatus.Done);
        todoItem.UpdatedAt.Should().NotBe(initialUpdatedAt);
        todoItem.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        todoItem.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void MarkAsTodo_ShouldUpdateStatusAndTimestamp()
    {
        // Arrange
        var todoItem = new TodoItem();
        todoItem.MarkAsCompleted();
        var initialUpdatedAt = todoItem.UpdatedAt;

        // Act
        todoItem.MarkAsTodo();

        // Assert
        todoItem.Status.Should().Be(TodoStatus.Todo);
        todoItem.UpdatedAt.Should().NotBe(initialUpdatedAt);
        todoItem.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        todoItem.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdatePropertiesAndTimestamp()
    {
        // Arrange
        var todoItem = new TodoItem();
        var title = "Updated Title";
        var description = "Updated Description";
        var dueDate = DateTime.UtcNow.AddDays(1);
        var initialUpdatedAt = todoItem.UpdatedAt;

        // Act
        todoItem.UpdateDetails(title, description, dueDate);

        // Assert
        todoItem.Title.Should().Be(title);
        todoItem.Description.Should().Be(description);
        todoItem.DueDate.Should().Be(dueDate);
        todoItem.UpdatedAt.Should().NotBe(initialUpdatedAt);
        todoItem.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateDetails_WithEmptyTitle_ShouldThrowArgumentException(string title)
    {
        // Arrange
        var todoItem = new TodoItem();

        // Act & Assert
        Action action = () => todoItem.UpdateDetails(title, "Description", DateTime.UtcNow.AddDays(1));

        action.Should().Throw<ArgumentException>()
            .WithMessage("Title cannot be empty");
    }

    [Fact]
    public void UpdateDetails_WithNullDescription_ShouldUpdateSuccessfully()
    {
        // Arrange
        var todoItem = new TodoItem();
        var title = "Valid Title";

        // Act
        todoItem.UpdateDetails(title, null, null);

        // Assert
        todoItem.Title.Should().Be(title);
        todoItem.Description.Should().BeNull();
        todoItem.DueDate.Should().BeNull();
    }

    [Fact]
    public void IsOverdue_WithPastDueDateAndNotCompleted_ShouldReturnTrue()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            DueDate = DateTime.UtcNow.AddDays(-1),
            Status = TodoStatus.Todo
        };

        // Act
        var result = todoItem.IsOverdue();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_WithPastDueDateAndCompleted_ShouldReturnFalse()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            DueDate = DateTime.UtcNow.AddDays(-1),
            Status = TodoStatus.Done
        };

        // Act
        var result = todoItem.IsOverdue();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WithFutureDueDate_ShouldReturnFalse()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            DueDate = DateTime.UtcNow.AddDays(1),
            Status = TodoStatus.Todo
        };

        // Act
        var result = todoItem.IsOverdue();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WithNoDueDate_ShouldReturnFalse()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            DueDate = null,
            Status = TodoStatus.Todo
        };

        // Act
        var result = todoItem.IsOverdue();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(TodoStatus.Todo, false)]
    [InlineData(TodoStatus.InProgress, false)]
    [InlineData(TodoStatus.Done, true)]
    public void IsCompleted_ShouldReturnCorrectValue(TodoStatus status, bool expectedResult)
    {
        // Arrange
        var todoItem = new TodoItem { Status = status };

        // Act
        var result = todoItem.IsCompleted;

        // Assert
        result.Should().Be(expectedResult);
    }
}
