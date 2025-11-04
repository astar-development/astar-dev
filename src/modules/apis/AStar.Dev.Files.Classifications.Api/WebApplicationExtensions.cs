using AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;
using Microsoft.AspNetCore.Builder;

namespace AStar.Dev.Files.Classifications.Api;

/// <summary>
///     Provides extension methods for configuring a <see cref="WebApplication" /> with file classification-related
///     services and endpoints.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    ///     Configures the application to include file classification-related services and endpoints.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication" /> instance to configure.</param>
    /// <returns>The <see cref="WebApplication" /> instance with file classification services configured.</returns>
    public static WebApplication UseFilesClassificationsApplicationServices(this WebApplication app)
    {
        app.MapFileClassificationsGetEndpoint();

        return app;
    }
}
