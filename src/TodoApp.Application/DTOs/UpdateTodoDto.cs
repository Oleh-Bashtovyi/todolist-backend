namespace TodoApp.Application.DTOs;

public record UpdateTodoDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime? DueDate
);