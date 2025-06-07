using Mapster;
using MediatR;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Commands.CreateTodoItem;

public class CreateTodoItemCommandHandler(ITodoRepository repository) : IRequestHandler<CreateTodoItemCommand, Guid>
{
    public async Task<Guid> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = request.TodoDto.Adapt<TodoItem>();

        var createdItem = await repository.AddAsync(todoItem, cancellationToken);

        return createdItem.Id;
    }
}