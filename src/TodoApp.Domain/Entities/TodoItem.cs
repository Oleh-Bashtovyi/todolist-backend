using TodoApp.Domain.Common;
using TodoApp.Domain.Enums;

namespace TodoApp.Domain.Entities;

public class TodoItem : BaseEntity, IAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TodoStatus Status { get; set; } = TodoStatus.Todo;
    public DateTime? DueDate { get; set; }
    public bool IsCompleted => Status == TodoStatus.Done;

    // Auditable properties
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Domain methods
    public void MarkAsInProgress()
    {
        Status = TodoStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        Status = TodoStatus.Done;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsTodo()
    {
        Status = TodoStatus.Todo;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string? description, DateTime? dueDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");

        Title = title;
        Description = description;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsOverdue()
    {
        return DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != TodoStatus.Done;
    }
}