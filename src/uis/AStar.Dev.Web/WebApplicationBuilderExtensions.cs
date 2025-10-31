using AStar.Dev.Api.Client.Sdk.Shared;
using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using AStar.Dev.ServiceDefaults;
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

        _ = builder.Services.AddOptions<FilesApiConfiguration>()
            .Bind(builder.Configuration.GetSection("apiConfiguration:filesApiConfiguration"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Existing raw scopes list
        var filesApiScopes = builder.Configuration
            .GetSection("apiConfiguration:filesApiConfiguration:Scopes")
            .Get<string[]>() ?? [];

        // Derive read/write scopes for FilesApiOptions
        _ = builder.Services.Configure<FilesApiConfiguration>(opts =>
                                                              {
                                                                  opts.ReadScopes = filesApiScopes
                                                                                    .Where(s => s.EndsWith("/ToDoList.Read", StringComparison.OrdinalIgnoreCase))
                                                                                    .Distinct()
                                                                                    .ToArray();

                                                                  opts.WriteScopes = filesApiScopes
                                                                                     .Where(s => s.EndsWith("/ToDoList.Write", StringComparison.OrdinalIgnoreCase))
                                                                                     .Distinct()
                                                                                     .ToArray();
                                                              });

        // Typed client registration (existing pattern). The FilesApiClient now consumes FilesApiOptions via IOptions<T>.
        builder.Services.AddApiClient<FilesApiClient, FilesApiConfiguration>(filesApiScopes);

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

        _ = builder.Services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();

        return builder;
    }
}
