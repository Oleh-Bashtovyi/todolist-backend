﻿using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using TodoApp.API.Middleware;

namespace TodoApp.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                // configure to deserialize enums as strings in JSON requests and responses
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: true));
            });

        services.AddEndpointsApiExplorer();
        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TodoApp API",
                Version = "v1",
                Description = "Todo application API"
            });

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });


        services.AddCors(options =>
        {
            var reactOrigins = configuration.GetSection("ReactAppUrls").Get<string[]>();

            options.AddPolicy("AllowReactApp", builder =>
            {
                builder
                    .WithOrigins(reactOrigins ?? [])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        services.AddHealthChecks();

        return services;
    }

    public static WebApplication UseApiServices(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoApp API V1");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseExceptionHandler(options => { });
        app.UseHttpsRedirection();
        app.UseCors("AllowReactApp");
        app.UseAuthorization();
        app.MapControllers();
        app.UseHealthChecks("/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

        return app;
    }
}