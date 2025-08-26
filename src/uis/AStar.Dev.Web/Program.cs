using AStar.Dev.ServiceDefaults;
using AStar.Dev.Web.Components;
using AStar.Dev.Web.Components.Layout.Menu;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
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

// Public by default; only pages/components with [Authorize] should challenge.
// Public by default; only pages/components with [Authorize] should challenge.
builder.Services.AddAuthorization();

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IMenuItemsService, MenuItemsService>();
builder.Services.AddBlazorBootstrap();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePagesWithRedirects("/404");

app.MapHealthChecks("/health");

if(!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/unexpected-error", true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

// Static assets should remain public
app.MapStaticAssets().AllowAnonymous();

// Map the Microsoft Identity UI endpoints so SignIn/SignOut URLs work
app.MapControllers().AllowAnonymous();

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
