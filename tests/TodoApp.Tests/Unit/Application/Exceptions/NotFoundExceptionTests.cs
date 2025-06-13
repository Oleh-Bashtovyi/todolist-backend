using TodoApp.Application.Exceptions;

namespace TodoApp.Tests.Unit.Application.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void NotFoundException_WithNameAndKey_ShouldFormatMessageCorrectly()
    {
        // Arrange
        var name = "TodoItem";
        var key = Guid.NewGuid();

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Message.Should().Be($"Entity \"{name}\" (key = {key}) was not found.");
    }

    [Fact]
    public void NotFoundException_WithStringKey_ShouldFormatMessageCorrectly()
    {
        // Arrange
        var name = "User";
        var key = "john.doe@example.com";

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Message.Should().Be($"Entity \"{name}\" (key = {key}) was not found.");
    }
}