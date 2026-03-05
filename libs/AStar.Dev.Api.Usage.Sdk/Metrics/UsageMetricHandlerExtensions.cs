using Microsoft.AspNetCore.Builder;

namespace AStar.Dev.Api.Usage.Sdk.Metrics;

/// <summary>
/// </summary>
public static class UsageMetricHandlerExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static void UseMetrics(this IApplicationBuilder builder)
        => builder.UseMiddleware<UsageMetricHandler>();
}
