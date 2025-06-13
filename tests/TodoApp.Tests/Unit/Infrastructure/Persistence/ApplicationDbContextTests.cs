namespace TodoApp.Tests.Unit.Infrastructure.Persistence;

public class ApplicationDbContextTests : IDisposable
{
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }

    [Fact]
    public void TodoItems_DbSet_ShouldBeConfigured()
    {
        // Act & Assert
        _context.TodoItems.Should().NotBeNull();
        _context.TodoItems.Should().BeAssignableTo<DbSet<TodoItem>>();
    }

    [Fact]
    public async Task SaveChangesAsync_WithAuditableEntity_ShouldSetCreatedAtForNewEntity()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Todo",
            Description = "Test Description",
            Status = TodoStatus.Todo
        };

        // Act
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        // Assert
        var savedItem = await _context.TodoItems.FindAsync(todoItem.Id);
        savedItem.Should().NotBeNull();
        savedItem!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        savedItem.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task TodoItem_Configuration_ShouldApplyCorrectConstraints()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = new string('A', 200), // Max length
            Description = new string('B', 1000), // Max length
            Status = TodoStatus.Todo
        };

        // Act & Assert
        _context.TodoItems.Add(todoItem);
        var saveAction = () => _context.SaveChangesAsync();
        await saveAction.Should().NotThrowAsync();
    }

    [Fact]
    public async Task TodoItem_WithNullTitle_ShouldThrowException()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = null!, // Required field
            Status = TodoStatus.Todo
        };

        // Act & Assert
        _context.TodoItems.Add(todoItem);
        var saveAction = () => _context.SaveChangesAsync();
        await saveAction.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task TodoItem_StatusEnum_ShouldBeStoredAsString()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Todo",
            Status = TodoStatus.InProgress
        };

        // Act
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        // Assert
        var savedItem = await _context.TodoItems.FindAsync(todoItem.Id);
        savedItem.Should().NotBeNull();
        savedItem!.Status.Should().Be(TodoStatus.InProgress);
    }

    [Fact]
    public async Task TodoItem_IsCompleted_ShouldBeIgnoredInDatabase()
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Todo",
            Status = TodoStatus.Done
        };

        // Act
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        // Clear the context to ensure fresh load
        _context.ChangeTracker.Clear();

        // Assert
        var savedItem = await _context.TodoItems.FindAsync(todoItem.Id);
        savedItem.Should().NotBeNull();
        savedItem!.IsCompleted.Should().BeTrue(); // This is computed property

        // Verify that IsCompleted is not actually stored in database by checking entity configuration
        var entityType = _context.Model.FindEntityType(typeof(TodoItem));
        var isCompletedProperty = entityType?.FindProperty(nameof(TodoItem.IsCompleted));
        isCompletedProperty.Should().BeNull(); // Should be ignored
    }

    [Theory]
    [InlineData(TodoStatus.Todo)]
    [InlineData(TodoStatus.InProgress)]
    [InlineData(TodoStatus.Done)]
    public async Task TodoItem_AllStatusValues_ShouldBeSavedAndRetrieved(TodoStatus status)
    {
        // Arrange
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = $"Test Todo - {status}",
            Status = status
        };

        // Act
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        // Assert
        var savedItem = await _context.TodoItems.FindAsync(todoItem.Id);
        savedItem.Should().NotBeNull();
        savedItem!.Status.Should().Be(status);
    }

    [Fact]
    public async Task TodoItem_WithDueDate_ShouldStoreDateTimeProperly()
    {
        // Arrange
        var dueDate = new DateTime(2024, 12, 25, 15, 30, 0, DateTimeKind.Utc);
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Todo with Due Date",
            Status = TodoStatus.Todo,
            DueDate = dueDate
        };

        // Act
        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        // Assert
        var savedItem = await _context.TodoItems.FindAsync(todoItem.Id);
        savedItem.Should().NotBeNull();
        savedItem!.DueDate.Should().Be(dueDate);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}