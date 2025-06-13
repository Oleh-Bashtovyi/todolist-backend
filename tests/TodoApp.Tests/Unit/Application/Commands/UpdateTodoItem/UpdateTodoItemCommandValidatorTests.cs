using FluentValidation.TestHelper;
using TodoApp.Application.DTOs;
using TodoApp.Application.Features.Commands.UpdateTodoItem;

namespace TodoApp.Tests.Unit.Application.Commands.UpdateTodoItem;

public class UpdateTodoItemCommandValidatorTests
{
    private readonly UpdateTodoItemCommandValidator _validator = new();

    [Fact]
    public void Validator_WithValidCommand_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UpdateTodoItemCommand(
            new UpdateTodoDto(Guid.NewGuid(), "Valid Title", "Valid Description", DateTime.UtcNow.AddDays(1)));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WithEmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTodoItemCommand(
            new UpdateTodoDto(Guid.Empty, "Valid Title", "Description", DateTime.UtcNow.AddDays(1)));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TodoDto.Id)
            .WithErrorMessage("Id is required.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validator_WithEmptyTitle_ShouldHaveValidationError(string title)
    {
        // Arrange
        var command = new UpdateTodoItemCommand(
            new UpdateTodoDto(Guid.NewGuid(), title, "Description", DateTime.UtcNow.AddDays(1)));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TodoDto.Title)
            .WithErrorMessage("Title is required.");
    }
}