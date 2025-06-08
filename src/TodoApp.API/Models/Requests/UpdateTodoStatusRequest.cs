using TodoApp.Domain.Enums;

namespace TodoApp.API.Models.Requests;

public record UpdateTodoStatusRequest(TodoStatus Status);