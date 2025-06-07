using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Queries.GetAllTodoItems;

public record GetAllTodoItemsQuery : IRequest<IEnumerable<TodoDto>>;