using Microsoft.EntityFrameworkCore;
using TodoApp.Application.Interfaces;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;

namespace TodoApp.Infrastructure.Repositories;

public class TodoRepository(IApplicationDbContext context) : ITodoRepository
{
    public async Task<TodoItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.TodoItems
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.TodoItems
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoItem> AddAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        var entityEntry = await context.TodoItems.AddAsync(todoItem, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entityEntry.Entity;
    }

    public async Task UpdateAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        context.TodoItems.Update(todoItem);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(TodoItem todoItem, CancellationToken cancellationToken = default)
    {
        context.TodoItems.Remove(todoItem);
        var affectedRows = await context.SaveChangesAsync(cancellationToken);
        return affectedRows > 0;
    }

    public async Task<IEnumerable<TodoItem>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        var currentDate = DateTime.UtcNow;

        return await context.TodoItems
            .Where(x => x.DueDate.HasValue &&
                       x.DueDate.Value < currentDate &&
                       x.Status != TodoStatus.Done)
            .OrderBy(x => x.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TodoItem>> GetByStatusAsync(TodoStatus status, CancellationToken cancellationToken = default)
    {
        return await context.TodoItems
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}