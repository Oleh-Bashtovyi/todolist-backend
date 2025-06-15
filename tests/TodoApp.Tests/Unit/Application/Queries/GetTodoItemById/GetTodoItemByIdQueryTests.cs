using TodoApp.Application.Features.Queries.GetTodoItemById;

namespace TodoApp.Tests.Unit.Application.Queries.GetTodoItemById;

public class GetTodoItemByIdQueryTests
{
    [Fact]
    public void Constructor_ShouldCreateQuery_WithValidId()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var query = new GetTodoItemByIdQuery(id);

        // Assert
        query.Id.Should().Be(id);
    }

    [Fact]
    public void Query_ShouldBeRecord_WithEqualityComparison()
    {
        // Arrange
        var id = Guid.NewGuid();
        var query1 = new GetTodoItemByIdQuery(id);
        var query2 = new GetTodoItemByIdQuery(id);

        // Act & Assert
        query1.Should().Be(query2);
    }
}
