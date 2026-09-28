using System.ComponentModel.DataAnnotations;
namespace WorkItems.Api.Models;
public sealed class WorkItem
{
    public int Id { get; set; }
    [Required, MaxLength(120)] public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public WorkItemStatus Status { get; set; } = WorkItemStatus.Todo;
    public DateTimeOffset CreatedAt { get; set; }
}
public enum WorkItemStatus { Todo, InProgress, Done }
