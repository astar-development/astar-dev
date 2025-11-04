using AStar.Dev.Aspire.Common;
using AStar.Dev.AspNet.Extensions.Handlers;
using AStar.Dev.AspNet.Extensions.WebApplicationBuilderExtensions;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace AStar.Dev.Files.Classifications.Api;

/// <summary>
/// </summary>
public class Class1
{
    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static async Task DoStuff()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        _ = builder.AddServiceDefaults();

        builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.AStarDb);

        _ = builder
            .DisableServerHeader();

        _ = builder.Services.AddOpenApi();
        _ = builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        _ = builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = ctx => ctx.ProblemDetails.Extensions.Add("nodeId", Environment.MachineName));

        _ = builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        _ = builder.Services.AddHealthChecks();

        WebApplication app = builder.Build();

        HeaderPolicyCollection policyCollection = new HeaderPolicyCollection()
            .AddDefaultSecurityHeaders()
            .AddPermissionsPolicyWithDefaultSecureDirectives();

        _ = app.UseSecurityHeaders(policyCollection);

        _ = app.MapHealthChecks("/health").ShortCircuit();

        // Configure the HTTP request pipeline.
        if(app.Environment.IsDevelopment())
        {
            _ = app.MapOpenApi();
            _ = app.MapScalarApiReference();
        }

        _ = app.UseHttpsRedirection();

        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        _ = app.MapGet("/weatherforecast", () =>
            {
                WeatherForecast[] forecast = Enumerable.Range(1, 5).Select(index =>
                        new WeatherForecast
                        (
                            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                            Random.Shared.Next(-20, 55),
                            summaries[Random.Shared.Next(summaries.Length)]
                        ))
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast");

        _ = app.MapShortCircuit(404, "robots.txt", "favicon.ico", "404.html", "sitemap.xml");

        _ = app.UseExceptionHandler();
        await Task.Delay(1);
    }

    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
