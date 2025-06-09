using System.ComponentModel;

namespace TodoApp.Domain.Enums;

public enum TodoStatus
{
    [Description("Pending task")]
    Todo = 0,

    [Description("Task in progress")]
    InProgress = 1,

    [Description("Completed task")]
    Done = 2
}