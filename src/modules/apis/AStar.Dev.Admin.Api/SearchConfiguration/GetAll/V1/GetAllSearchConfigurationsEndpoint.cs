using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.SearchConfiguration.GetAll.V1;

/// <summary>
///     The <see cref="GetAllSearchConfigurationsEndpoint" /> class contains the <see cref="AddEndpoint" /> method.
/// </summary>
/// <param name="app">The instance of <see cref="WebApplication" /> to add the endpoint to</param>
public sealed class GetAllSearchConfigurationsEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SearchConfigurationGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SearchConfigurationEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/",
                   async (HttpContext context, AdminContext adminContext, CancellationToken cancellationToken) =>
                       await Handle(new GetAllSearchConfigurationsQuery(context.User), adminContext, cancellationToken))
           .AddBasicProduces<IEnumerable<GetAllSearchConfigurationsResponse>>()
           .WithDescription("Get all the available Search Configurations")
           .WithSummary("Get all Search Configurations")

            // .RequireAuthorization()
           .WithTags(EndpointConstants.SearchConfigurationTag);
    }

    private async Task<IResult> Handle(GetAllSearchConfigurationsQuery request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        List<Infrastructure.AdminDb.Models.SearchConfiguration> config = await adminContext
                                                                              .SearchConfiguration.GroupBy(l => l.SiteConfigurationSlug)
                                                                              .Select(g => g.OrderByDescending(c => c.UpdatedOn).First())
                                                                              .ToListAsync(cancellationToken);

        IEnumerable<GetAllSearchConfigurationsResponse> data = config.Select(x => new GetAllSearchConfigurationsResponse(x));

        return TypedResults.Ok(data);
    }
}
