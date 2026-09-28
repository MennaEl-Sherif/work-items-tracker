using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkItems.Api.Contracts;
using WorkItems.Api.Data;
using WorkItems.Api.Models;
namespace WorkItems.Api.Controllers;
[ApiController]
[Route("api/work-items")]
public sealed class WorkItemsController(WorkItemsDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<WorkItemResponse>> Create(CreateWorkItemRequest request, CancellationToken token)
    {
        var title = request.Title?.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            ModelState.AddModelError(nameof(request.Title), "Title is required."); return ValidationProblem(ModelState);
        }
        var item = new WorkItem 
        {   Title = title,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(), 
            Status = WorkItemStatus.Todo, 
            CreatedAt = DateTimeOffset.UtcNow };
        db.WorkItems.Add(item);
        try
        {
            await db.SaveChangesAsync(token);
        }
        catch (DbUpdateException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Database write failed",
                detail: exception.GetBaseException().Message);
        }
        return Created($"/api/work-items/{item.Id}", ToResponse(item));
    }
    [HttpGet]
    public async Task<ActionResult<PagedResult<WorkItemResponse>>> Get([FromQuery] string? title, [FromQuery] WorkItemStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
    {
        if (page < 1 || pageSize is < 1 or > 100) { ModelState.AddModelError("pagination", "Page must be at least 1 and pageSize must be between 1 and 100."); return ValidationProblem(ModelState); }
        var query = db.WorkItems.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(title)) 
        { 
            var term = title.Trim(); 
            query = query.Where(x => x.Title.Contains(term));
        }
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        var total = await query.CountAsync(token);
        var items = await query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(x => ToResponse(x)).ToListAsync(token);
        return Ok(new PagedResult<WorkItemResponse>(items, page, pageSize, total, total == 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize)));
    }
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<WorkItemResponse>> UpdateStatus(int id, UpdateStatusRequest request, CancellationToken token)
    {
        if (!request.Status.HasValue) { ModelState.AddModelError(nameof(request.Status), "Status is required."); return ValidationProblem(ModelState); }
        var item = await db.WorkItems.FindAsync([id], token);
        if (item is null) 
            return NotFound();
        var expected = item.Status 
            switch 
        { WorkItemStatus.Todo => WorkItemStatus.InProgress, WorkItemStatus.InProgress => WorkItemStatus.Done, _ => (WorkItemStatus?)null };
        if (request.Status != expected)
            return Conflict(new ProblemDetails { Status = 409, Title = "Invalid status transition", Detail = $"Cannot change status from {item.Status} to {request.Status}." });
        item.Status = request.Status.Value;
        await db.SaveChangesAsync(token);
        return Ok(ToResponse(item));
    }
    private static WorkItemResponse ToResponse(WorkItem item) => new(item.Id, item.Title, item.Description, item.Status, item.CreatedAt);
}
