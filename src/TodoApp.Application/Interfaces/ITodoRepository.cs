using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Application.Interfaces;

public interface ITodoRepository
{
    public Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<TodoItem> AddAsync(TodoItem todoItem, CancellationToken cancellationToken = default);
    public Task UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken = default);
    public Task<bool> DeleteAsync(TodoItem todoItem, CancellationToken cancellationToken = default);
    public Task<IEnumerable<TodoItem>> GetOverdueAsync(CancellationToken cancellationToken = default);
    public Task<IEnumerable<TodoItem>> GetByStatusAsync(TodoStatus status, CancellationToken cancellationToken = default);
}