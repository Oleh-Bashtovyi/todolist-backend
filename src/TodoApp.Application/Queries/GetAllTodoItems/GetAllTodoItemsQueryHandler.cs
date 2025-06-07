using Mapster;
using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;

namespace TodoApp.Application.Queries.GetAllTodoItems;

public class GetAllTodoItemsQueryHandler(ITodoRepository repository)
    : IRequestHandler<GetAllTodoItemsQuery, IEnumerable<TodoDto>>
{
    public async Task<IEnumerable<TodoDto>> Handle(GetAllTodoItemsQuery request, CancellationToken cancellationToken)
    {
        var todoItems = await repository.GetAllAsync(cancellationToken);

        return todoItems.Adapt<IEnumerable<TodoDto>>();
    }
}