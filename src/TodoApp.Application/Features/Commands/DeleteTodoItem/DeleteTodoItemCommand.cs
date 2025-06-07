using MediatR;

namespace TodoApp.Application.Features.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(Guid Id) : IRequest<bool>;
