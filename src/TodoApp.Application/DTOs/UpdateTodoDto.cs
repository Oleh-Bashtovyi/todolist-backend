namespace TodoApp.Application.DTOs;

public record UpdateTodoDto(
    string Title,
    string? Description,
    DateTime? DueDate
);