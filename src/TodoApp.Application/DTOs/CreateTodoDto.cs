using TodoApp.Domain.Enums;

namespace TodoApp.Application.DTOs;

public record CreateTodoDto(
    string Title,
    string? Description,
    DateTime? DueDate,
    TodoStatus Status = TodoStatus.Todo
);