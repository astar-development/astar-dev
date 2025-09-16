using AStar.Dev.ServiceDefaults;
using AStar.Dev.Web.Components;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using OpenTelemetry.Resources;

namespace AStar.Dev.Web;

/// <summary>
///     Provides extension methods for configuring the <see cref="WebApplicationBuilder" /> to use
///     predefined application services, such as OpenTelemetry, Azure Monitor,
///     Fluent UI, authentication, authorization, and health checks.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    ///     Configures and adds the default application services, including OpenTelemetry, Azure Monitor,
    ///     Razor Components, Fluent UI, authentication, authorization, and health checks, to the specified
    ///     <see cref="WebApplicationBuilder" />.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder" /> to configure the application services for.</param>
    /// <returns>The same <see cref="WebApplicationBuilder" /> instance with the default services configured and added.</returns>
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        var dictionary = new Dictionary<string, object> { { "service.name", "AStar.Dev.Web" }, { "service.namespace", "AStar.Dev.Web" } };

        builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
                                                            {
                                                                options.ConnectionString = builder.Configuration["AzureMonitor:ConnectionString"];
                                                            })
               .ConfigureResource(resourceBuilder => resourceBuilder.AddAttributes(dictionary));

        builder.AddServiceDefaults();

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

        return builder;
    }
}
