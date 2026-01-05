using System.Security.Claims;
using Asp.Versioning.Builder;
using AStar.Dev.Api.Usage.Sdk;
using AStar.Dev.Infrastructure.UsageDb.Data;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Usage.Logger.Usage.GetAll.V1;

/// <summary>
///     The <see cref="GetAllApiUsageEventsEndpoint" /> class contains the <see cref="AddEndpoint" /> method
/// </summary>
/// <param name="app">The instance of <see cref="WebApplication" /> to add the endpoint to</param>
public sealed class GetAllApiUsageEventsEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.UsageGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.UsageEndpoint)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapGet("/",
                   async (HttpContext context, ApiUsageContext usageContext, ILoggerAstar<GetAllApiUsageEventsEndpoint> logger, CancellationToken cancellationToken) => await Handle(context.User,
                                                                                                                                                                                     usageContext,
                                                                                                                                                                                     logger,
                                                                                                                                                                                     cancellationToken))
           .AddBasicProduces<Dictionary<string, List<ApiUsageEvent>>>()
           .WithDescription("Gets all of the API Usage Metrics - no filter at the moment")
           .WithSummary("Get all the API Usage Metrics")

           // .RequireAuthorization()
           .WithTags(EndpointConstants.UsageTag);
    }

    private async Task<IResult> Handle(ClaimsPrincipal user, ApiUsageContext usageContext, ILoggerAstar<GetAllApiUsageEventsEndpoint> logger, CancellationToken cancellationToken)
    {
        if(!user.Claims.FirstOrDefault(c => c.Type == "name")!.Value.Contains("Jason Barden"))
        {
            logger.LogDebug("Username: {Username}", user.Claims.Where(c => c.Type == "name"));

            return Results.Forbid();
        }

        List<ApiUsageEvent> config = await usageContext
                                          .ApiUsage.ToListAsync(cancellationToken);

        var groupedEvents = config
                           .GroupBy(x => x.ApiName)
                           .ToDictionary(grouping => grouping.Key, detail => detail.OrderBy(usage => usage.ApiEndpoint).ThenByDescending(s => s.Timestamp).Take(10));

        logger.LogDebug("Total API Usage Events: {ApiUsageEventCount}", config.Count);

        return TypedResults.Ok(groupedEvents);
    }
}
