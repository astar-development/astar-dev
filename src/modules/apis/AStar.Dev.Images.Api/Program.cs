using AStar.Dev.AspNet.Extensions.Handlers;
using AStar.Dev.Images.Api;
using AStar.Dev.ServiceDefaults;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddOpenApi();
_ = builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
_ = builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = ctx => ctx.ProblemDetails.Extensions.Add("nodeId", Environment.MachineName));

_ = builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHealthChecks();

var app = builder.Build();

var policyCollection = new HeaderPolicyCollection()
                       .AddDefaultSecurityHeaders()
                       .AddPermissionsPolicyWithDefaultSecureDirectives();

app.UseSecurityHeaders(policyCollection);

app.MapHealthChecks("/health").ShortCircuit();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
    _ = app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

app.MapGet("/weatherforecast", () =>
                               {
                                   var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                                                                                    (
                                                                                     DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                                                                     Random.Shared.Next(-20, 55),
                                                                                     summaries[Random.Shared.Next(summaries.Length)]
                                                                                    ))
                                                            .ToArray();

                                   return forecast;
                               })
   .WithName("GetWeatherForecast");

app.MapShortCircuit(404, "robots.txt", "favicon.ico", "404.html", "sitemap.xml");


_ = app.UseExceptionHandler();
await app.RunAsync();

namespace AStar.Dev.Images.Api
{
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
