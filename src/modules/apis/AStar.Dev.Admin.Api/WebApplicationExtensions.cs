using Microsoft.AspNetCore.Builder;

namespace AStar.Dev.Admin.Api;

/// <summary>
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static WebApplication UseAdminApiApplicationServices(this WebApplication app)
        => app; // app.MapFileClassificationsGetEndpoint();
}
