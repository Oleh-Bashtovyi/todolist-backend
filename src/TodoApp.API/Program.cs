using TodoApp.API;
using TodoApp.Application;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Data;
using TodoApp.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DatabaseInitializer.InitializeAsync(context);
}

app.Run();

public partial class Program { }