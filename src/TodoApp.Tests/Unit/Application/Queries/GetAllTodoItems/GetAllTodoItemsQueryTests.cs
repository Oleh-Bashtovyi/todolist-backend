using TodoApp.Application.Features.Queries.GetAllTodoItems;

namespace TodoApp.Tests.Unit.Application.Queries.GetAllTodoItems;

public class GetAllTodoItemsQueryTests
{
    [Fact]
    public void Query_ShouldBeRecord_WithEqualityComparison()
    {
        // Arrange
        var query1 = new GetAllTodoItemsQuery();
        var query2 = new GetAllTodoItemsQuery();

        // Act & Assert
        query1.Should().Be(query2);
    }
}