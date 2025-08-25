using AStar.Dev.ServiceDefaults;
using AStar.Dev.Web.Components;
using AStar.Dev.Web.Components.Layout.Menu;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using OpenTelemetry.Resources;

var resourceAttributes = new Dictionary<string, object> { { "service.name", "AStar.Dev.Web" }, { "service.namespace", "AStar.Dev.Web" } };
var builder            = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
                                                    {
                                                        options.ConnectionString = builder.Configuration["AzureMonitor:ConnectionString"];
                                                    })
       .ConfigureResource(resourceBuilder => resourceBuilder.AddAttributes(resourceAttributes));

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

builder.Services
       .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
       .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
                                  {
                                      // Require authentication by default; opt-out with [AllowAnonymous] on specific pages/components
                                      options.FallbackPolicy = options.DefaultPolicy;
                                  });

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IMenuItemsService, MenuItemsService>();
builder.Services.AddBlazorBootstrap();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithRedirects("/404");

app.MapHealthChecks("/health");

if(!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets().AllowAnonymous();

// Map the Blazor app endpoints; allow anonymous transport. Page-level [Authorize]/[AllowAnonymous] still applies.
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode()
   .AllowAnonymous();

await app.RunAsync();

namespace AStar.Dev.Web
{
    public class Program
    {
    }
}
