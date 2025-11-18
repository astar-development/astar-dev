// using AStar.Dev.Files.Api.Endpoints.Add.V1;
// using AStar.Dev.Files.Api.Endpoints.Get.V1;
// using AStar.Dev.Files.Classifications.Api;
// using AStar.Dev.ServiceDefaults;

using AStar.Dev.Web.Components;
using Scalar.AspNetCore;

namespace AStar.Dev.Web;

/// <summary>
///     Provides extension methods for <see cref="WebApplicationBuilder" /> to enhance or modify configurations.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    ///     Removes the default "Server" header from HTTP responses to improve security
    ///     by preventing disclosure of the web server's software.
    ///     This extension method should be called last, right before the call to Build() to ensure no other middleware adds the server header back...
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder" /> instance to configure.</param>
    /// <returns>The configured <see cref="WebApplicationBuilder" /> instance.</returns>
    public static WebApplicationBuilder RemoveServerHeader(this WebApplicationBuilder builder)
    {
        _ = builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

        return builder;
    }

    /// <summary>
    ///     Removes the default "Server" header from HTTP responses to enhance security by preventing server software disclosure,
    ///     and proceeds to build the <see cref="WebApplication" /> instance.
    ///     This method encompasses both the header removal and the final application building for streamlined configuration.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder" /> instance to configure.</param>
    /// <returns>A fully configured and built <see cref="WebApplication" /> instance.</returns>
    public static WebApplication RemoveServerHeaderAndBuild(this WebApplicationBuilder builder) => builder.RemoveServerHeader().Build();

    /// <summary>
    ///     Configures the web application to use various application services such as security headers, health checks,
    ///     exception handling, authentication, authorization, and routing components like static files and controllers.
    ///     This method ensures the required middleware and features are added to the application's request pipeline.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication" /> instance to configure.</param>
    /// <returns>The configured <see cref="WebApplication" /> instance.</returns>
    /// <example>
    ///     Example usage:
    ///     <code>
    /// var app = builder.Build();
    /// app.UseApplicationServices();
    /// await app.RunAsync();
    /// </code>
    /// </example>
    public static WebApplication UseApplicationServices(this WebApplication app)
    {
        _ = app.UseExceptionHandler("/Error", true);

        if (!app.Environment.IsDevelopment())
            _ = app.UseHsts();

        _ = app.UseHttpsRedirection();

        var policyCollection = new HeaderPolicyCollection()
            .AddDefaultSecurityHeaders()
            .AddPermissionsPolicyWithDefaultSecureDirectives();

        _ = app.UseSecurityHeaders(policyCollection);

        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        _ = app.UseAntiforgery();

        _ = app.MapStaticAssets();

        _ = app.MapControllers();

        _ = app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        _ = app.MapOpenApi();
        _ = app.MapScalarApiReference();

        _ = app.MapDefaultEndpoints();
        //
        // app.MapFilesPostEndpoint();
        // app.MapFilesGetEndpoint();
        // _ = app.UseFilesClassificationsApplicationServices();
        _ = app.UseExceptionHandler();

        _ = app.MapShortCircuit(404, "robots.txt", "favicon.ico", "404.html", "sitemap.xml");

        return app;
    }
}
