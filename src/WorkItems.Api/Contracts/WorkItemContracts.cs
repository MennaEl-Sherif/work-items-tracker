using System.ComponentModel.DataAnnotations;
using WorkItems.Api.Models;
namespace WorkItems.Api.Contracts;
public sealed record CreateWorkItemRequest([Required, MaxLength(120)] string Title, string? Description);
public sealed record UpdateStatusRequest([Required] WorkItemStatus? Status);
public sealed record WorkItemResponse(int Id, string Title, string? Description, WorkItemStatus Status, DateTimeOffset CreatedAt);
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);
