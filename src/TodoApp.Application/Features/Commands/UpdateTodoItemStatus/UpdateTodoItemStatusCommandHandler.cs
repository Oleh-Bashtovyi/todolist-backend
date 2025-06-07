using Mapster;
using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Features.Commands.UpdateTodoItemStatus;

public class UpdateTodoItemStatusCommandHandler(ITodoRepository repository)
    : IRequestHandler<UpdateTodoItemStatusCommand, TodoDto>
{
    public async Task<TodoDto> Handle(UpdateTodoItemStatusCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await repository.GetByIdAsync(request.Id, cancellationToken)
                       ?? throw new NotFoundException(nameof(TodoItem), request.Id);

        switch (request.Status)
        {
            case TodoStatus.Todo:
                todoItem.MarkAsTodo();
                break;
            case TodoStatus.InProgress:
                todoItem.MarkAsInProgress();
                break;
            case TodoStatus.Done:
                todoItem.MarkAsCompleted();
                break;
        }

        await repository.UpdateAsync(todoItem, cancellationToken);
        return todoItem.Adapt<TodoDto>();
    }
}