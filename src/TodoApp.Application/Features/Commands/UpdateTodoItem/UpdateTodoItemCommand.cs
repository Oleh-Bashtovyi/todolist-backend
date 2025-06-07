using FluentValidation;
using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Features.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand(UpdateTodoDto TodoDto) : IRequest<TodoDto>;

public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    public UpdateTodoItemCommandValidator()
    {
        RuleFor(x => x.TodoDto.Id)
            .NotNull().WithMessage("Id is required.");

        RuleFor(x => x.TodoDto.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.TodoDto.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

/*        RuleFor(x => x.TodoDto.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
            .When(x => x.TodoDto.DueDate.HasValue);*/
    }
}