using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Enums;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Seed;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();
        await SeedAsync(context);
    }

    private static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!await context.TodoItems.AnyAsync())
        {
            var todoItems = new List<TodoItem>
            {
                new() 
                { 
                    Id = new Guid("58c49479-ec65-4de2-86e7-033cccbbbba1"), 
                    Title = "Sample Todo 1", 
                    Description = "This is a sample todo item."
                },
                new()
                {
                    Id = new Guid("58c49479-ec65-4de2-86e7-033cccbbbba2"),
                    Title = "Sample Todo 2", 
                    Description = "This is another sample todo item.",
                    Status = TodoStatus.InProgress
                },
                new()
                {
                    Id = new Guid("58c49479-ec65-4de2-86e7-033cccbbbba3"),
                    Title = "Sample Todo 3",
                    Description = "Ehe another sample todo item.",
                    Status = TodoStatus.Done,
                    DueDate = DateTime.UtcNow.AddDays(5)
                }
            };
            await context.TodoItems.AddRangeAsync(todoItems);
            await context.SaveChangesAsync();
        }
    }
}