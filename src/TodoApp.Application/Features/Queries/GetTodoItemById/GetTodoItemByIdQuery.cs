using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Features.Queries.GetTodoItemById;

public record GetTodoItemByIdQuery(Guid Id) : IRequest<TodoDto>;