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
        builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

        return builder;
    }

    /// <summary>
    ///     Removes the default "Server" header from HTTP responses to enhance security by preventing server software disclosure,
    ///     and proceeds to build the <see cref="WebApplication" /> instance.
    ///     This method encompasses both the header removal and the final application building for streamlined configuration.
    /// </summary>
    /// <param name="app">The <see cref="WebApplicationBuilder" /> instance to configure and build.</param>
    /// <returns>A fully configured and built <see cref="WebApplication" /> instance.</returns>
    public static WebApplication RemoveServerHeaderAndBuild(this WebApplicationBuilder app) => app.RemoveServerHeader().Build();
}
