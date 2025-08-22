using AStar.Dev.ServiceDefaults;
using AStar.Dev.Web.Components;
using AStar.Dev.Web.Services;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Resources;

var resourceAttributes = new Dictionary<string, object> { { "service.name", "my-service" }, { "service.namespace", "my-namespace" }, { "service.instance.id", "my-instance" } };
var builder            = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
                                                    {
                                                        options.ConnectionString = builder.Configuration["AzureMonitor:ConnectionString"];
                                                    })
       .ConfigureResource(resourceBuilder => resourceBuilder.AddAttributes(resourceAttributes));

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<MenuItemsService>();
builder.Services.AddBlazorBootstrap();

var app = builder.Build();

app.MapHealthChecks("/health");

if(!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync();

namespace AStar.Dev.Web
{
    public class Program
    {
    }
}
