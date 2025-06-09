using TodoApp.Domain.Enums;

namespace TodoApp.Application.DTOs;

public record TodoDto(
    Guid Id,
    string Title,
    string? Description,
    TodoStatus Status,
    DateTime? DueDate,
    bool IsCompleted,
    bool IsOverdue,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);