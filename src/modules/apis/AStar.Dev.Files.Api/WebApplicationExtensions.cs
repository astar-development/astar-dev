using Microsoft.AspNetCore.Builder;

namespace AStar.Dev.Files.Api;

/// <summary>
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseApplicationServices(this WebApplication app)
        => app; // app.MapFileClassificationsGetEndpoint();
}
