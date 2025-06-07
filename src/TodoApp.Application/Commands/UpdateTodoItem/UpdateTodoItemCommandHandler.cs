using Mapster;
using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Commands.UpdateTodoItem;

public class UpdateTodoItemCommandHandler(ITodoRepository repository) : IRequestHandler<UpdateTodoItemCommand, TodoDto>
{
    public async Task<TodoDto> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = await repository.GetByIdAsync(request.TodoDto.Id, cancellationToken) 
                       ?? throw new NotFoundException(nameof(TodoItem), request.TodoDto.Id);

        todoItem.UpdateDetails(request.TodoDto.Title, request.TodoDto.Description, request.TodoDto.DueDate);

        await repository.UpdateAsync(todoItem, cancellationToken);

        return todoItem.Adapt<TodoDto>();
    }
}