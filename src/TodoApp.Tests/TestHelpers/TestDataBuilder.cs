using TodoApp.Application.DTOs;
using TodoApp.Application.Features.Commands.CreateTodoItem;

namespace TodoApp.Tests.TestHelpers;

public class TestDataBuilder
{
    private readonly Fixture _fixture;

    public TestDataBuilder()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public TodoItem CreateTodoItem(
        string? title = null,
        string? description = null,
        TodoStatus status = TodoStatus.Todo,
        DateTime? deadline = null)
    {
        return new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title ?? _fixture.Create<string>(),
            Description = description ?? _fixture.Create<string>(),
            Status = status,
            DueDate = deadline,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public List<TodoItem> CreateTodoItems(int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => CreateTodoItem())
            .ToList();
    }

    public CreateTodoItemCommand CreateTodoItemCommand(
        string? title = null,
        string? description = null,
        DateTime? deadline = null)
    {
        return new CreateTodoItemCommand(new CreateTodoDto(
            title ?? _fixture.Create<string>(),
            description ?? _fixture.Create<string>(),
            deadline));
    }
}