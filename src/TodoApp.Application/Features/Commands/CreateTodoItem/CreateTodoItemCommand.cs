using FluentValidation;
using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Features.Commands.CreateTodoItem;

public record CreateTodoItemCommand(CreateTodoDto TodoDto) : IRequest<Guid>;

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(x => x.TodoDto.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.TodoDto.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

/*        RuleFor(x => x.TodoDto.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
            .When(x => x.TodoDto.DueDate.HasValue);*/

        RuleFor(x => x.TodoDto.Status)
            .IsInEnum().WithMessage("Status must be a valid TodoStatus value.");
    }
}