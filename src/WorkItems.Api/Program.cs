using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WorkItems.Api.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDbContext<WorkItemsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("WorkItems")));
builder.Services.AddCors(options => options.AddPolicy("Angular", policy => policy.WithOrigins(builder.Configuration["ClientOrigin"] ?? "http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddProblemDetails();
var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler();
app.UseCors("Angular");
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<WorkItemsDbContext>().Database.MigrateAsync();
}
app.Run();
public partial class Program;
