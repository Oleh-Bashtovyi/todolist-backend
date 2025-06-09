using Mapster;
using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Application.Exceptions;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.Queries.GetTodoItemById;

public class GetTodoItemByIdQueryHandler(ITodoRepository repository) : IRequestHandler<GetTodoItemByIdQuery, TodoDto>
{
    public async Task<TodoDto> Handle(GetTodoItemByIdQuery request, CancellationToken cancellationToken)
    {
        var todoItem = await repository.GetByIdAsync(request.Id, cancellationToken)
                       ?? throw new NotFoundException(nameof(TodoItem), request.Id);

        return todoItem.Adapt<TodoDto>();
    }
}