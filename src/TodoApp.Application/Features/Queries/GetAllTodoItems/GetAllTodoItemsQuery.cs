using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Features.Queries.GetAllTodoItems;

public record GetAllTodoItemsQuery : IRequest<IEnumerable<TodoDto>>;