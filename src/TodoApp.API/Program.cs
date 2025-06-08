using TodoApp.API;
using TodoApp.Application;
using TodoApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApiServices();

app.Run();
