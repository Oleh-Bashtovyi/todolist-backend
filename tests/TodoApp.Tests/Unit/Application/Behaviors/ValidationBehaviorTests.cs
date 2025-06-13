using FluentValidation;
using FluentValidation.Results;
using MediatR;
using TodoApp.Application.Behaviors;

namespace TodoApp.Tests.Unit.Application.Behaviors;

public class ValidationBehaviorTests
{
    private readonly Mock<IValidator<TestRequest>> _validatorMock;
    private readonly ValidationBehavior<TestRequest, TestResponse> _behavior;
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _nextMock;

    public ValidationBehaviorTests()
    {
        _validatorMock = new Mock<IValidator<TestRequest>>();
        _behavior = new ValidationBehavior<TestRequest, TestResponse>(new[] { _validatorMock.Object });
        _nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationPasses()
    {
        // Arrange
        var request = new TestRequest("Valid");
        var response = new TestResponse("Success");
        var validationResult = new ValidationResult();

        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _nextMock.Setup(x => x.Invoke(CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        _nextMock.Verify(x => x.Invoke(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        // Arrange
        var request = new TestRequest("Invalid");
        var validationFailure = new ValidationFailure("Value", "Value is invalid");
        var validationResult = new ValidationResult([validationFailure]);

        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _behavior.Handle(request, _nextMock.Object, CancellationToken.None));

        exception.Errors.Should().Contain(validationFailure);
        _nextMock.Verify(x => x.Invoke(CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldProceed_WhenNoValidatorsExist()
    {
        // Arrange
        var request = new TestRequest("Test");
        var response = new TestResponse("Success");
        var behaviorWithoutValidators = new ValidationBehavior<TestRequest, TestResponse>(Enumerable.Empty<IValidator<TestRequest>>());

        _nextMock.Setup(x => x.Invoke(CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await behaviorWithoutValidators.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        _nextMock.Verify(x => x.Invoke(CancellationToken.None), Times.Once);
    }

    public record TestRequest(string Value) : IRequest<TestResponse>;
    public record TestResponse(string Value);
}