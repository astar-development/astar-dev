using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.SearchConfiguration.GetBySlug.V1;

public sealed class GetSearchConfigurationBySlugEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SearchConfigurationGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SearchConfigurationEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/{slug}",
                   async (string slug, HttpContext context, AdminContext adminContext, CancellationToken cancellationToken) =>
                       await Handle(new GetSearchConfigurationBySlugQuery(slug, context.User), adminContext, cancellationToken))
           .WithDescription("This method returns a single search configuration that matches the specified site slug.")
           .WithSummary("Get the search configuration matching the given slug")
           .WithName("GetSearchConfigurationBySlug")

            // .RequireAuthorization()
           .AddBasicProduces<GetSearchConfigurationBySlugResponse>()
           .Produces(404)
           .WithTags(EndpointConstants.SearchConfigurationTag);
    }

    private async Task<IResult> Handle(GetSearchConfigurationBySlugQuery request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        Infrastructure.AdminDb.Models.SearchConfiguration? config = await adminContext
                                                                         .SearchConfiguration
                                                                         .OrderByDescending(siteConfig => siteConfig.UpdatedOn)
                                                                         .Where(site => site.SiteConfigurationSlug == request.Slug)
                                                                         .FirstOrDefaultAsync(cancellationToken);

        return config is null
                   ? Results.NotFound()
                   : Results.Ok(new GetSearchConfigurationBySlugResponse(config));
    }
}
