using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoApp.Application.DTOs;
using TodoApp.Application.Interfaces;

namespace TodoApp.Tests.Integration.Controllers;

public class TodoApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly ITodoRepository _todoService;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public TodoApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        });

        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _todoService = _scope.ServiceProvider.GetRequiredService<ITodoRepository>();
    }

    [Fact]
    public async Task GetAllTodos_ShouldReturnEmptyList_WhenNoTodosExist()
    {
        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var todos = JsonSerializer.Deserialize<List<TodoDto>>(jsonResponse, _jsonOptions);

        todos.Should().NotBeNull();
        todos.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAndGetTodo_ShouldWorkEndToEnd()
    {
        // Arrange
        var createTodoDto = new CreateTodoDto(
            "Integration Test Todo",
            "This is a test todo created via integration test",
            DateTime.UtcNow.AddDays(7)
        );

        // Act - Create Todo
        var createResponse = await _client.PostAsJsonAsync("/api/todos", createTodoDto);

        // Assert - Create succeeded
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTodoJson = await createResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoDto>(createdTodoJson, _jsonOptions);

        createdTodo.Should().NotBeNull();
        createdTodo!.Title.Should().Be(createTodoDto.Title);
        createdTodo.Description.Should().Be(createTodoDto.Description);
        createdTodo.Status.Should().Be(TodoStatus.Todo);
        createdTodo.Id.Should().NotBeEmpty();

        // Act - Get the created Todo by ID
        var getResponse = await _client.GetAsync($"/api/todos/{createdTodo.Id}");

        // Assert - Get succeeded and returns the same todo
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTodoJson = await getResponse.Content.ReadAsStringAsync();
        var retrievedTodo = JsonSerializer.Deserialize<TodoDto>(retrievedTodoJson, _jsonOptions);

        retrievedTodo.Should().NotBeNull();
        retrievedTodo.Should().BeEquivalentTo(createdTodo);

        // Act - Get all todos should now include our created todo
        var getAllResponse = await _client.GetAsync("/api/todos");

        // Assert - GetAll includes our todo
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var allTodosJson = await getAllResponse.Content.ReadAsStringAsync();
        var allTodos = JsonSerializer.Deserialize<List<TodoDto>>(allTodosJson, _jsonOptions);

        allTodos.Should().NotBeNull();
        allTodos.Should().HaveCount(1);
        allTodos!.First().Should().BeEquivalentTo(createdTodo);
    }

    [Fact]
    public async Task UpdateTodoStatusWorkflow_ShouldWorkEndToEnd()
    {
        // Arrange - Create a todo first
        var createTodoDto = new CreateTodoDto(
            "Status Update Test Todo",
            "This todo will have its status updated",
            DateTime.UtcNow.AddDays(3)
        );

        var createResponse = await _client.PostAsJsonAsync("/api/todos", createTodoDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTodoJson = await createResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoDto>(createdTodoJson, _jsonOptions);

        // Act 1 - Update status to InProgress
        var updateStatusRequest = new { Status = TodoStatus.InProgress };
        var updateResponse = await _client.PatchAsJsonAsync($"/api/todos/{createdTodo!.Id}/status", updateStatusRequest);

        // Assert 1 - Status update succeeded
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTodoJson = await updateResponse.Content.ReadAsStringAsync();
        var updatedTodo = JsonSerializer.Deserialize<TodoDto>(updatedTodoJson, _jsonOptions);

        updatedTodo.Should().NotBeNull();
        updatedTodo!.Status.Should().Be(TodoStatus.InProgress);
        updatedTodo.Id.Should().Be(createdTodo.Id);
        updatedTodo.Title.Should().Be(createdTodo.Title);

        // Act 2 - Update status to Done
        var finalUpdateRequest = new { Status = TodoStatus.Done };
        var finalUpdateResponse = await _client.PatchAsJsonAsync($"/api/todos/{createdTodo.Id}/status", finalUpdateRequest);

        // Assert 2 - Final status update succeeded
        finalUpdateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var finalTodoJson = await finalUpdateResponse.Content.ReadAsStringAsync();
        var finalTodo = JsonSerializer.Deserialize<TodoDto>(finalTodoJson, _jsonOptions);

        finalTodo.Should().NotBeNull();
        finalTodo!.Status.Should().Be(TodoStatus.Done);

        // Act 3 - Verify the todo still exists with correct status
        var verifyResponse = await _client.GetAsync($"/api/todos/{createdTodo.Id}");

        // Assert 3 - Todo exists with Done status
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var verifiedTodoJson = await verifyResponse.Content.ReadAsStringAsync();
        var verifiedTodo = JsonSerializer.Deserialize<TodoDto>(verifiedTodoJson, _jsonOptions);

        verifiedTodo.Should().NotBeNull();
        verifiedTodo!.Status.Should().Be(TodoStatus.Done);
    }

    [Fact]
    public async Task GetNonExistentTodo_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/todos/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTodoStatus_WithInvalidStatus_ShouldReturnBadRequest()
    {
        // Arrange - Create a todo first
        var createTodoDto = new CreateTodoDto("Test Todo", "Description", null);
        var createResponse = await _client.PostAsJsonAsync("/api/todos", createTodoDto);
        var createdTodoJson = await createResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoDto>(createdTodoJson, _jsonOptions);

        // Act - Try to update with invalid status
        var invalidUpdateRequest = new { Status = 999 }; // Invalid status value
        var updateResponse = await _client.PatchAsJsonAsync($"/api/todos/{createdTodo!.Id}/status", invalidUpdateRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public void Dispose()
    {
        var allTodos = _todoService.GetAllAsync().Result;
        foreach (var todo in allTodos)
        {
            _todoService.DeleteAsync(todo).Wait();
        }

        _scope?.Dispose();
        _client?.Dispose();
    }
}