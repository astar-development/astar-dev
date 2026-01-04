using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.SiteConfigurations.Update.V1;

/// <summary>
///     The <see cref="UpdateSiteConfigurationEndpoint" /> class contains the <see cref="AddEndpoint" /> method
/// </summary>
/// <param name="app">The instance of <see cref="WebApplication" /> to add the endpoint to</param>
public sealed class UpdateSiteConfigurationEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SiteConfigurationGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SiteConfigurationsEndpoint)
                                    .HasDeprecatedApiVersion(0.9)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapPut("/{slug}",
                   async ([FromRoute] string slug,
                          HttpContext context,
                          [FromBody] UpdateSiteConfigurationCommand updateSiteConfigurationCommand,
                          AdminContext adminContext,
                          CancellationToken cancellationToken) => await Handle(new UpdateSiteConfigurationCommandForDb(slug, context.User, updateSiteConfigurationCommand), adminContext,
                                                                               cancellationToken))
           .WithDescription("Update the site configuration matching the specified ScrapeDirectoryId")
           .WithSummary("Update the site configuration")
           .AddBasicWithAdditionalProduces<UpdateSiteConfigurationResponse>()

           // .RequireAuthorization()
           .WithTags(EndpointConstants.SiteConfigurationTag)
           .MapToApiVersion(1.0);
    }

    private async Task<IResult> Handle(UpdateSiteConfigurationCommandForDb request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        SiteConfiguration? config = await adminContext
                                         .SiteConfiguration
                                         .OrderByDescending(siteConfig => siteConfig.UpdatedOn)
                                         .Where(site => site.SiteConfigurationSlug == request.Slug)
                                         .FirstOrDefaultAsync(cancellationToken);

        if(config is null) return TypedResults.NotFound();

        var insertConfig = new SiteConfiguration
                           {
                               BaseUrl               = request.BaseUrl,
                               LoginUrl              = request.LoginUrl,
                               Password              = request.Password,
                               SiteConfigurationSlug = request.Slug,
                               UpdatedOn             = DateTime.UtcNow,
                               UpdatedBy             = request.UpdatedBy,
                               LoginEmailAddress     = request.LoginEmailAddress,
                               Username              = request.Username,
                           };

        _ = adminContext.SiteConfiguration.Add(insertConfig);
        _ = await adminContext.SaveChangesAsync(cancellationToken);

        var apiVersion = new ApiVersion { Version = "1.0", };

        return TypedResults.CreatedAtRoute(new UpdateSiteConfigurationResponse
                                           {
                                               BaseUrl           = insertConfig.BaseUrl,
                                               LoginUrl          = insertConfig.LoginUrl,
                                               Password          = insertConfig.Password,
                                               LoginEmailAddress = insertConfig.LoginEmailAddress,
                                               Username          = insertConfig.Username,
                                               Slug              = insertConfig.SiteConfigurationSlug,
                                           }, "GetSiteConfigurationBySlug", apiVersion);
    }
}
