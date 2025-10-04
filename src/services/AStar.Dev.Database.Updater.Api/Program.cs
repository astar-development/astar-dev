using System.IO.Abstractions;
using AStar.Dev.Database.Updater.Api;
using AStar.Dev.Database.Updater.Api.FileKeywordProcessor;
using AStar.Dev.ServiceDefaults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

app.MapGet("/weatherforecast", () => {
                                   var forecast = Enumerable.Range(1, 5).Select(index =>
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

var policyCollection = new HeaderPolicyCollection()
                       .AddDefaultSecurityHeaders()
                       .AddPermissionsPolicyWithDefaultSecureDirectives();

app.UseSecurityHeaders(policyCollection);

app.MapGet("/process-files", async ([FromServices] FileScanner fileScanner, [FromServices] DatabaseWriter writer, [FromServices] IOptions<DatabaseUpdaterConfiguration> config,
                                    [FromServices] IFileSystem fileSystem) => {
                                 var enumerationOptions = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };
                                 var filePaths          = fileSystem.Directory.GetFiles(config.Value.RootDirectory, "*", enumerationOptions);
                                 var cts                = new CancellationTokenSource();

                                 var producer = fileScanner.ScanFilesAsync(filePaths, cts.Token);
                                 var consumer = writer.ConsumeAsync(cts.Token);

                                 await Task.WhenAll(producer, consumer);
                             });

app.MapDefaultEndpoints();

await app.RunAsync();

namespace AStar.Dev.Database.Updater.Api
{
    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
