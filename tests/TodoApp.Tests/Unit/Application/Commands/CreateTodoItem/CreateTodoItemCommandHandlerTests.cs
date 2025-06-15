using TodoApp.Application.DTOs;
using TodoApp.Application.Features.Commands.CreateTodoItem;
using TodoApp.Application.Interfaces;

namespace TodoApp.Tests.Unit.Application.Commands.CreateTodoItem;

public class CreateTodoItemCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly CreateTodoItemCommandHandler _handler;

    public CreateTodoItemCommandHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _handler = new CreateTodoItemCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateTodoItem()
    {
        // Arrange
        var createDto = new CreateTodoDto("Test Title", "Test Description", DateTime.UtcNow.AddDays(1));
        var command = new CreateTodoItemCommand(createDto);

        var createdTodoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = createDto.Title,
            Description = createDto.Description,
            DueDate = createDto.DueDate,
            Status = createDto.Status,
            CreatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTodoItem);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(createDto.Title);
        result.Description.Should().Be(createDto.Description);
        result.DueDate.Should().Be(createDto.DueDate);
        result.Status.Should().Be(createDto.Status);

        _repositoryMock.Verify(x => x.AddAsync(It.Is<TodoItem>(t =>
            t.Title == createDto.Title &&
            t.Description == createDto.Description &&
            t.DueDate == createDto.DueDate &&
            t.Status == createDto.Status), It.IsAny<CancellationToken>()), Times.Once);
    }
}
