using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Api.Usage.Sdk.Metrics;

/// <summary>
/// </summary>
/// <param name="next"></param>
/// <param name="logger"></param>
/// <param name="send"></param>
public sealed class UsageMetricHandler(RequestDelegate next, ILogger<UsageMetricHandler> logger, Send send)
{
    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var (httpRequest, httpMethod, apiEndpoint, apiName) = (context.Request, context.Request.Method, context.Request.Path, context.Request.Host.Host);

            apiName = UpdateApiNameIfRequired(apiName);

            var startTimestamp = Stopwatch.GetTimestamp();

            logger.LogDebug("In UsageMetricHandler > InvokeAsync with - request: {Request}, Method: {Method}, apiEndpoint: {ApiEndpoint}, apiName: {ApiName}",
                            httpRequest, httpMethod, apiEndpoint, apiName);

            await next(context);

            var endTimestamp = Stopwatch.GetTimestamp();
            var diff         = Stopwatch.GetElapsedTime(startTimestamp, endTimestamp);

            await send.SendUsageEventAsync(new(apiName, apiEndpoint, httpMethod, diff.TotalMilliseconds, context.Response.StatusCode), CancellationToken.None);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "An error occured");
        }
    }

    private string UpdateApiNameIfRequired(string apiName)
    {
        if(apiName != "host.docker.internal")
        {
            return apiName;
        }

        logger.LogDebug("Updating host.docker.internal - doubt this ever happens as 99.999999% sure this is a long gone requirement. I'm just not in the mood to dig now...");
        apiName = "astar.dev.images.api";

        return apiName;
    }
}
