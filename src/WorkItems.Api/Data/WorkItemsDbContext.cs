using Microsoft.EntityFrameworkCore;
using WorkItems.Api.Models;
namespace WorkItems.Api.Data;
public sealed class WorkItemsDbContext(DbContextOptions<WorkItemsDbContext> options) : DbContext(options)
{
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<WorkItem>();
        entity.Property(x => x.Title).HasMaxLength(120).IsRequired();
        entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        entity.HasIndex(x => x.CreatedAt);
    }
}
