using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.SiteConfigurations.GetBySlug.V1;

/// <summary>
///     The <see cref="GetSiteConfigurationBySlugEndpoint" /> class contains the <see cref="AddEndpoint" /> method
/// </summary>
/// <param name="app">The instance of <see cref="WebApplication" /> to add the endpoint to</param>
public sealed class GetSiteConfigurationBySlugEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SiteConfigurationGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SiteConfigurationsEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/{slug}",
                   async ([FromRoute] string slug, HttpContext context, AdminContext adminContext, CancellationToken cancellationToken) =>
                       await Handle(new GetSiteConfigurationBySlugQuery(slug, context.User), adminContext, cancellationToken))
           .WithDescription("This method returns a single site configuration that matches the specified site slug.")
           .WithSummary("Get site configuration by slug")
           .WithName("GetSiteConfigurationBySlug")

            // .RequireAuthorization()
           .AddBasicProduces<GetSiteConfigurationBySlugResponse>()
           .Produces(404)
           .WithTags(EndpointConstants.SiteConfigurationTag);
    }

    private async Task<IResult> Handle(GetSiteConfigurationBySlugQuery request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        SiteConfiguration? config = await adminContext
                                         .SiteConfiguration
                                         .OrderByDescending(siteConfig => siteConfig.UpdatedOn)
                                         .Where(site => site.SiteConfigurationSlug == request.Slug)
                                         .FirstOrDefaultAsync(cancellationToken);

        return config is null ? Results.NotFound() : Results.Ok(config.ToDto());
    }
}
