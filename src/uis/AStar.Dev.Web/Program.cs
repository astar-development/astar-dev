#pragma warning disable CA1506
using AStar.Dev.ServiceDefaults;
using AStar.Dev.Web;
using AStar.Dev.Web.Components;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.FluentUI.AspNetCore.Components;
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

// Add services to the container.
builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

builder.Services.AddFluentUIComponents();

builder.Services
       .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
       .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

builder.Services.AddControllersWithViews()
       .AddMicrosoftIdentityUI();

var app = builder.RemoveServerHeaderAndBuild();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


var policyCollection = new HeaderPolicyCollection()
                       .AddDefaultSecurityHeaders()
                       .AddPermissionsPolicyWithDefaultSecureDirectives();

app.UseSecurityHeaders(policyCollection);

// Auth middlewares BEFORE components
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Static files remain anonymous
app.MapStaticAssets();

// SignIn/SignOut endpoints
app.MapControllers();

// Do NOT require auth at the endpoint; enforce via Router
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync();
#pragma warning restore CA1506
