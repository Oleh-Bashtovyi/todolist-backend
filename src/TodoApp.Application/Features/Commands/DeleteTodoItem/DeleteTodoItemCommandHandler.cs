using MediatR;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.Commands.DeleteTodoItem;

public class DeleteTodoItemCommandHandler(ITodoRepository repository) : IRequestHandler<DeleteTodoItemCommand, bool>
{
    public async Task<bool> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await repository.GetByIdAsync(request.Id, cancellationToken)
                       ?? throw new NotFoundException(nameof(TodoItem), request.Id);

        return await repository.DeleteAsync(todoItem, cancellationToken);
    }
}