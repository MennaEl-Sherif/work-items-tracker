using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkItems.Api.Contracts;
using WorkItems.Api.Controllers;
using WorkItems.Api.Data;
using WorkItems.Api.Models;

namespace WorkItems.Api.Tests;

public sealed class WorkItemsControllerTests
{
    private static WorkItemsDbContext CreateDb() => new(new DbContextOptionsBuilder<WorkItemsDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task Create_TrimsTitleAndAlwaysStartsInTodo()
    {
        await using var db = CreateDb();
        var result = await new WorkItemsController(db).Create(new("  Ship it  ", "notes"), default);
        var created = Assert.IsType<CreatedResult>(result.Result);
        var item = Assert.IsType<WorkItemResponse>(created.Value);
        Assert.Equal("Ship it", item.Title);
        Assert.Equal(WorkItemStatus.Todo, item.Status);
        Assert.Single(db.WorkItems);
    }

    [Fact]
    public async Task Create_RejectsWhitespaceTitle()
    {
        await using var db = CreateDb();
        var result = await new WorkItemsController(db).Create(new("   ", null), default);
        Assert.IsType<ObjectResult>(result.Result);
        Assert.Empty(db.WorkItems);
    }

    [Fact]
    public async Task Get_FiltersByTitleAndStatusAndPaginates()
    {
        await using var db = CreateDb();
        db.WorkItems.AddRange(
            Item("Alpha report", WorkItemStatus.Todo, 3),
            Item("Alpha review", WorkItemStatus.Todo, 2),
            Item("Alpha done", WorkItemStatus.Done, 1),
            Item("Beta", WorkItemStatus.Todo, 0));
        await db.SaveChangesAsync();
        var result = await new WorkItemsController(db).Get("Alpha", WorkItemStatus.Todo, 1, 1);
        var page = Assert.IsType<OkObjectResult>(result.Result).Value as PagedResult<WorkItemResponse>;
        Assert.NotNull(page);
        Assert.Single(page.Items);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(2, page.TotalPages);
    }

    [Fact]
    public async Task UpdateStatus_AllowsOnlyNextStatus()
    {
        await using var db = CreateDb();
        var item = Item("Task", WorkItemStatus.Todo, 0);
        db.Add(item); await db.SaveChangesAsync();
        var controller = new WorkItemsController(db);
        var result = await controller.UpdateStatus(item.Id, new(WorkItemStatus.InProgress), default);
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(WorkItemStatus.InProgress, item.Status);
        result = await controller.UpdateStatus(item.Id, new(WorkItemStatus.Todo), default);
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsNotFoundForMissingItem()
    {
        await using var db = CreateDb();
        var result = await new WorkItemsController(db).UpdateStatus(404, new(WorkItemStatus.InProgress), default);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public async Task Get_RejectsInvalidPagination(int page, int pageSize)
    {
        await using var db = CreateDb();
        var result = await new WorkItemsController(db).Get(null, null, page, pageSize);
        Assert.IsType<ObjectResult>(result.Result);
    }

    private static WorkItem Item(string title, WorkItemStatus status, int minutesAgo) => new()
    { Title = title, Status = status, CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-minutesAgo) };
}
