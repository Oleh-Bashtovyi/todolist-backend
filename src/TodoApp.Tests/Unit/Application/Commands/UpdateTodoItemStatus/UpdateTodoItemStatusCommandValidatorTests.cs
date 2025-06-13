using FluentValidation.TestHelper;
using TodoApp.Application.Features.Commands.UpdateTodoItemStatus;

namespace TodoApp.Tests.Unit.Application.Commands.UpdateTodoItemStatus;

public class UpdateTodoItemStatusCommandValidatorTests
{
    private readonly UpdateTodoItemStatusCommandValidator _validator = new();

    [Theory]
    [InlineData(TodoStatus.Todo)]
    [InlineData(TodoStatus.InProgress)]
    [InlineData(TodoStatus.Done)]
    public void Validator_WithValidStatus_ShouldNotHaveValidationError(TodoStatus status)
    {
        // Arrange
        var command = new UpdateTodoItemStatusCommand(Guid.NewGuid(), status);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_WithInvalidStatus_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTodoItemStatusCommand(Guid.NewGuid(), (TodoStatus)999);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status must be a valid TodoStatus value.");
    }
}