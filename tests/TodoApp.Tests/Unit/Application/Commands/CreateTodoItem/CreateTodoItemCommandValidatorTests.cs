using FluentValidation.TestHelper;
using TodoApp.Application.DTOs;
using TodoApp.Application.Features.Commands.CreateTodoItem;

namespace TodoApp.Tests.Unit.Application.Commands.CreateTodoItem;

public class CreateTodoItemCommandValidatorTests
{
    private readonly CreateTodoItemCommandValidator _validator = new();

    [Fact]
    public void Validator_WithValidCommand_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateTodoItemCommand(
            new CreateTodoDto("Valid Title", "Valid Description", DateTime.UtcNow.AddDays(1)));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validator_WithEmptyTitle_ShouldHaveValidationError(string title)
    {
        // Arrange
        var command = new CreateTodoItemCommand(
            new CreateTodoDto(title, "Description", DateTime.UtcNow.AddDays(1)));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TodoDto.Title);
    }

    [Fact]
    public void Validator_WithTitleTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var longTitle = new string('A', 201);
        var command = new CreateTodoItemCommand(
            new CreateTodoDto(longTitle, "Description", DateTime.UtcNow.AddDays(1)));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TodoDto.Title);
    }

    [Fact]
    public void Validator_WithDescriptionTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var longDescription = new string('A', 1001);
        var command = new CreateTodoItemCommand(
            new CreateTodoDto("Valid Title", longDescription, DateTime.UtcNow.AddDays(1)));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TodoDto.Description);
    }

    [Fact]
    public void Validator_WithInvalidStatus_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateTodoItemCommand(
            new CreateTodoDto("Valid Title", "Description", DateTime.UtcNow.AddDays(1), (TodoStatus)999));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TodoDto.Status);
    }

    [Fact]
    public void Validator_WithNullDescription_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new CreateTodoItemCommand(
            new CreateTodoDto("Valid Title", null, DateTime.UtcNow.AddDays(1)));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.TodoDto.Description);
    }
}