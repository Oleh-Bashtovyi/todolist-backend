using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Behaviors;

namespace TodoApp.Tests.Unit.Application.Behaviors;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>> _loggerMock;
    private readonly LoggingBehavior<TestRequest, TestResponse> _behavior;
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _nextMock;

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();
        _behavior = new LoggingBehavior<TestRequest, TestResponse>(_loggerMock.Object);
        _nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();
    }

    [Fact]
    public async Task Handle_ShouldLogStartAndEnd_WhenRequestIsProcessed()
    {
        // Arrange
        var request = new TestRequest("Test");
        var response = new TestResponse("Response");
        _nextMock.Setup(x => x.Invoke(CancellationToken.None)).ReturnsAsync(response);

        // Act
        var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        _nextMock.Verify(x => x.Invoke(CancellationToken.None), Times.Once);

        // Verify start log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("[START]")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        // Verify end log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("[END]")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldLogWarning_WhenRequestTakesMoreThanThreeSeconds()
    {
        // Arrange
        var request = new TestRequest("Test");
        var response = new TestResponse("Response");
        _nextMock.Setup(x => x.Invoke(CancellationToken.None)).Returns(async () =>
        {
            await Task.Delay(3100); // More than 3 seconds
            return response;
        });

        // Act
        var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

        // Assert
        result.Should().Be(response);

        // Verify performance warning log
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("[PERFORMANCE]")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    public record TestRequest(string Value) : IRequest<TestResponse>;
    public record TestResponse(string Value);
}
