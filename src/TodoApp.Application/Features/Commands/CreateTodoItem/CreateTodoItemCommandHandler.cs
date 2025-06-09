using Mapster;
using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.Commands.CreateTodoItem;

public class CreateTodoItemCommandHandler(ITodoRepository repository) : IRequestHandler<CreateTodoItemCommand, TodoDto>
{
    public async Task<TodoDto> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var todoItem = request.TodoDto.Adapt<TodoItem>();

        var createdItem = await repository.AddAsync(todoItem, cancellationToken);

        var createdDto = createdItem.Adapt<TodoDto>();

        return createdDto;
    }
}