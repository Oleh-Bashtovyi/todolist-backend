using TodoApp.Application.Features.Queries.GetAllTodoItems;
using TodoApp.Application.Interfaces;

namespace TodoApp.Tests.Unit.Application.Queries.GetAllTodoItems;

public class GetAllTodoItemsQueryHandlerTests
{
    private readonly Mock<ITodoRepository> _repositoryMock;
    private readonly GetAllTodoItemsQueryHandler _handler;

    public GetAllTodoItemsQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITodoRepository>();
        _handler = new GetAllTodoItemsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllTodoItems_WhenItemsExist()
    {
        // Arrange
        var query = new GetAllTodoItemsQuery();
        var todoItems = new List<TodoItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "First Item",
                Description = "First Description",
                Status = TodoStatus.Todo,
                DueDate = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Second Item",
                Description = "Second Description",
                Status = TodoStatus.InProgress,
                DueDate = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoItems);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Title == "First Item");
        result.Should().Contain(x => x.Title == "Second Item");

        _repositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyCollection_WhenNoItemsExist()
    {
        // Arrange
        var query = new GetAllTodoItemsQuery();
        var emptyList = new List<TodoItem>();

        _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _repositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
