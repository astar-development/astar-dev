using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.SiteConfigurations.GetAll.V1;

/// <summary>
///     The <see cref="GetAllSiteConfigurationsEndpoint" /> class contains the <see cref="AddEndpoint" /> method
/// </summary>
/// <param name="app">The instance of <see cref="WebApplication" /> to add the endpoint to</param>
public sealed class GetAllSiteConfigurationsEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SiteConfigurationGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SiteConfigurationsEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/",
                   async (HttpContext context, AdminContext adminContext, CancellationToken cancellationToken) =>
                       await Handle(new GetAllSiteConfigurationsQuery(context.User), adminContext, cancellationToken))
           .AddBasicProduces<IEnumerable<GetAllSiteConfigurationsResponse>>()
           .WithDescription("Gets all of the site configurations available")
           .WithSummary("Get all site configurations")

            // .RequireAuthorization()
           .WithTags(EndpointConstants.SiteConfigurationTag);
    }

    private async Task<IResult> Handle(GetAllSiteConfigurationsQuery request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        List<SiteConfiguration> config = await adminContext
                                              .SiteConfiguration.GroupBy(l => l.SiteConfigurationSlug)
                                              .Select(g => g.OrderByDescending(c => c.UpdatedOn).First())
                                              .ToListAsync(cancellationToken);

        if(!request.User.HasClaim(c => c.Value == "jason.barden@outlook.com")) config.ForEach(configuration => configuration.Password = "Sorry, you're not authorised to access this property.");

        var data = config.Select(x => x.ToDto()).ToList();

        return TypedResults.Ok(data);
    }
}
