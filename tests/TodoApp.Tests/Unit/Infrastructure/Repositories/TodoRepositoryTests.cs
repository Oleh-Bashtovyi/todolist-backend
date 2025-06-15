using TodoApp.Infrastructure.Repositories;

namespace TodoApp.Tests.Unit.Infrastructure.Repositories;

public class TodoRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly TodoRepository _repository;

    public TodoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new TodoRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnTodoItem()
    {
        // Arrange
        var todoItem = CreateTodoItem();
        await _context.TodoItems.AddAsync(todoItem);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(todoItem.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(todoItem.Id);
        result.Title.Should().Be(todoItem.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithMultipleItems_ShouldReturnOrderedByCreatedAtDescending()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            CreateTodoItem("First", TodoStatus.Todo, DateTime.UtcNow.AddDays(-2)),
            CreateTodoItem("Second", TodoStatus.Todo, DateTime.UtcNow.AddDays(-1)),
            CreateTodoItem("Third", TodoStatus.Todo, DateTime.UtcNow)
        };

        await _context.TodoItems.AddRangeAsync(todoItems);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDatabase_ShouldReturnEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddAsync_WithValidTodoItem_ShouldAddAndReturnTodoItem()
    {
        // Arrange
        var todoItem = CreateTodoItem();

        // Act
        var result = await _repository.AddAsync(todoItem);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Title.Should().Be(todoItem.Title);

        var dbItem = await _context.TodoItems.FindAsync(result.Id);
        dbItem.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingTodoItem_ShouldUpdateItem()
    {
        // Arrange
        var todoItem = CreateTodoItem();
        await _context.TodoItems.AddAsync(todoItem);
        await _context.SaveChangesAsync();

        todoItem.Title = "Updated Title";
        todoItem.Description = "Updated Description";

        // Act
        await _repository.UpdateAsync(todoItem);

        // Assert
        var updatedItem = await _context.TodoItems.FindAsync(todoItem.Id);
        updatedItem.Should().NotBeNull();
        updatedItem!.Title.Should().Be("Updated Title");
        updatedItem.Description.Should().Be("Updated Description");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingTodoItem_ShouldReturnTrueAndRemoveItem()
    {
        // Arrange
        var todoItem = CreateTodoItem();
        await _context.TodoItems.AddAsync(todoItem);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(todoItem);

        // Assert
        result.Should().BeTrue();
        var deletedItem = await _context.TodoItems.FindAsync(todoItem.Id);
        deletedItem.Should().BeNull();
    }

    [Fact]
    public async Task GetOverdueAsync_WithOverdueItems_ShouldReturnOnlyOverdueItems()
    {
        // Arrange
        var overdueItem1 = CreateTodoItem("Overdue 1", status: TodoStatus.Todo, dueDate: DateTime.UtcNow.AddDays(-2));
        var overdueItem2 = CreateTodoItem("Overdue 2", status: TodoStatus.InProgress, dueDate: DateTime.UtcNow.AddDays(-1));
        var notOverdueItem = CreateTodoItem("Not Overdue", status: TodoStatus.Todo, dueDate: DateTime.UtcNow.AddDays(1));
        var completedOverdueItem = CreateTodoItem("Completed Overdue", status: TodoStatus.Done, dueDate: DateTime.UtcNow.AddDays(-1));
        var itemWithoutDueDate = CreateTodoItem("No Due Date", status: TodoStatus.Todo);

        await _context.TodoItems.AddRangeAsync(overdueItem1, overdueItem2, notOverdueItem, completedOverdueItem, itemWithoutDueDate);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetOverdueAsync()).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(item => item.Title == "Overdue 1");
        result.Should().Contain(item => item.Title == "Overdue 2");
        result.Should().NotContain(item => item.Title == "Not Overdue");
        result.Should().NotContain(item => item.Title == "Completed Overdue");
        result.Should().NotContain(item => item.Title == "No Due Date");
    }

    [Fact]
    public async Task GetByStatusAsync_WithSpecificStatus_ShouldReturnItemsWithThatStatus()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            CreateTodoItem("Todo 1", status: TodoStatus.Todo),
            CreateTodoItem("Todo 2", status: TodoStatus.Todo),
            CreateTodoItem("In Progress", status: TodoStatus.InProgress),
            CreateTodoItem("Done", status: TodoStatus.Done)
        };

        await _context.TodoItems.AddRangeAsync(todoItems);
        await _context.SaveChangesAsync();

        // Act
        var todoResult = (await _repository.GetByStatusAsync(TodoStatus.Todo)).ToList();
        var inProgressResult = (await _repository.GetByStatusAsync(TodoStatus.InProgress)).ToList();
        var doneResult = (await _repository.GetByStatusAsync(TodoStatus.Done)).ToList();

        // Assert
        todoResult.Should().HaveCount(2);
        todoResult.All(item => item.Status == TodoStatus.Todo).Should().BeTrue();

        inProgressResult.Should().HaveCount(1);
        inProgressResult.First().Title.Should().Be("In Progress");

        doneResult.Should().HaveCount(1);
        doneResult.First().Title.Should().Be("Done");
    }

    [Fact]
    public async Task GetByStatusAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _repository.GetByStatusAsync(TodoStatus.Todo, cts.Token));
    }

    private static TodoItem CreateTodoItem(
        string title = "Test Todo",
        TodoStatus status = TodoStatus.Todo,
        DateTime? dueDate = null,
        DateTime? createdAt = null)
    {
        return new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = "Test Description",
            Status = status,
            DueDate = dueDate,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}