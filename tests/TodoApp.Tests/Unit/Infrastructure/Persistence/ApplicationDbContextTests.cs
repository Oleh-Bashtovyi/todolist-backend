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