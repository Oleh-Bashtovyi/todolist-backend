using FluentValidation;
using MediatR;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Features.Commands.UpdateTodoItemStatus;

public record UpdateTodoItemStatusCommand(Guid Id, TodoStatus Status) : IRequest<TodoDto>;

public class UpdateTodoItemStatusCommandValidator : AbstractValidator<UpdateTodoItemStatusCommand>
{
    public UpdateTodoItemStatusCommandValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid TodoStatus value.");
    }
}
