// using Asp.Versioning;
// using AStar.Dev.AspNet.Extensions.Handlers;
// using AStar.Dev.Files.Classifications.Api;
// using AStar.Dev.ServiceDefaults;

using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using OpenTelemetry.Resources;

namespace AStar.Dev.Web;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        var dictionary = new Dictionary<string, object>
        {
            { "service.name", "AStar.Dev.Web" },
            { "service.namespace", "AStar.Dev.Web" }
        };

        _ = builder.Services.AddOpenTelemetry()
            .UseAzureMonitor(o => o.ConnectionString = builder.Configuration["AzureMonitor:ConnectionString"])
            .ConfigureResource(r => r.AddAttributes(dictionary));

        _ = builder.AddServiceDefaults();
        _ = builder.Services.AddCascadingAuthenticationState();

        _ = builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddAuthenticationStateSerialization(o => o.SerializeAllClaims = true);

        _ = builder.Services.AddFluentUIComponents();
        _ = builder.Services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

        _ = builder.Services.AddAuthorization(options => options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")));
        _ = builder.Services.AddHealthChecks();
        // _ = builder.Services.AddOpenApi();

        _ = builder.Services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();
        // _ = builder.Services.AddScoped<IFileClassificationsService, FileClassificationsService>();
        // _ = builder.AddFileClassificationsApplicationServices();
        // _ = builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        _ = builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails =
            ctx => ctx.ProblemDetails.Extensions.Add("nodeId", Environment.MachineName));

        return builder;
    }
}
