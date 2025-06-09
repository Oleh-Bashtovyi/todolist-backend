using FluentValidation;
using MediatR;
using TodoApp.Application.DTOs;

namespace TodoApp.Application.Features.Commands.CreateTodoItem;

public record CreateTodoItemCommand(CreateTodoDto TodoDto) : IRequest<TodoDto>;

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(x => x.TodoDto.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.TodoDto.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.TodoDto.Status)
            .IsInEnum().WithMessage("Status must be a valid TodoStatus value.");
    }
}