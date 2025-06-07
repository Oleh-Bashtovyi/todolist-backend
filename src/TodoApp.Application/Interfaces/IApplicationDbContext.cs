using TodoApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.Application.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<TodoItem> TodoItems { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}