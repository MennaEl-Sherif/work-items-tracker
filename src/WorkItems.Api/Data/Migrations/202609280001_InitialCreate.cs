using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
#nullable disable
namespace WorkItems.Api.Data.Migrations;
[DbContext(typeof(WorkItemsDbContext))]
[Migration("202609280001_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "WorkItems", columns: table => new
        {
            Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
            Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
            CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_WorkItems", x => x.Id));
        migrationBuilder.CreateIndex(name: "IX_WorkItems_CreatedAt", table: "WorkItems", column: "CreatedAt");
    }
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "WorkItems");
}
