using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.API.Models.Requests;
using TodoApp.Application.DTOs;
using TodoApp.Application.Features.Commands.CreateTodoItem;
using TodoApp.Application.Features.Commands.DeleteTodoItem;
using TodoApp.Application.Features.Commands.UpdateTodoItem;
using TodoApp.Application.Features.Commands.UpdateTodoItemStatus;
using TodoApp.Application.Features.Queries.GetAllTodoItems;
using TodoApp.Application.Features.Queries.GetTodoItemById;
using TodoApp.Domain.Enums;

namespace TodoApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController(IMediator mediator, ILogger<TodosController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all todos");

        var query = new GetAllTodoItemsQuery();
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoDto>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting todo with id: {TodoId}", id);

        var query = new GetTodoItemByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result == null)
        {
            logger.LogWarning("Todo with id {TodoId} not found", id);
            return NotFound($"Todo with id {id} not found");
        }

        return Ok(result);
    }

/*    [HttpGet("by-status/{status}")]
    [ProducesResponseType(typeof(IEnumerable<TodoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetByStatus(TodoStatus status, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting todos with status: {Status}", status);

        var query = new GetTodosByStatusQuery(status);
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }*/

    [HttpPost]
    [ProducesResponseType(typeof(TodoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoDto>> Create([FromBody] CreateTodoDto createTodoDto, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new todo with title: {Title}", createTodoDto.Title);

        var command = new CreateTodoItemCommand(createTodoDto);
        var result = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoDto>> Update([FromBody] UpdateTodoDto updateTodoDto, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating todo with id: {TodoId}", updateTodoDto.Id);

        var command = new UpdateTodoItemCommand(updateTodoDto);
        var result = await mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoDto>> UpdateStatus(Guid id, [FromBody] UpdateTodoStatusRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating status for todo {TodoId} to {Status}", id, request.Status);

        var command = new UpdateTodoItemStatusCommand(id, request.Status);
        var result = await mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting todo with id: {TodoId}", id);

        var command = new DeleteTodoItemCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return Ok(new { Deleted = result });
    }
}