using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
#nullable disable
namespace WorkItems.Api.Data.Migrations;
[DbContext(typeof(WorkItemsDbContext))]
partial class WorkItemsDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.21");
        modelBuilder.Entity("WorkItems.Api.Models.WorkItem", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int").HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
            b.Property<DateTimeOffset>("CreatedAt").HasColumnType("datetimeoffset");
            b.Property<string>("Description").HasColumnType("nvarchar(max)");
            b.Property<string>("Status").IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)");
            b.Property<string>("Title").IsRequired().HasMaxLength(120).HasColumnType("nvarchar(120)");
            b.HasKey("Id"); b.HasIndex("CreatedAt"); b.ToTable("WorkItems");
        });
    }
}
