using AStar.Dev.Admin.Api.SiteConfigurations.GetAll.V1;
using AStar.Dev.Infrastructure.AdminDb.Models;

namespace AStar.Dev.Admin.Api.SiteConfigurations;

/// <summary>
///     The SiteConfigurationExtensions class contains the applicable extension methods for the
///     <see cref="SiteConfiguration" /> class
/// </summary>
public static class SiteConfigurationExtensions
{
    /// <summary>
    ///     The ToDto method will convert the <see cref="Infrastructure.AdminDb.Models.SiteConfiguration" /> object to the
    ///     <see cref="GetAllSiteConfigurationsResponse" /> onject to be returned to the caller
    /// </summary>
    /// <param name="siteConfiguration">The instance of <see cref="Infrastructure.AdminDb.Models.SiteConfiguration" /></param>
    /// to convert to DTO
    /// <returns>The configured instance of <see cref="GetAllSiteConfigurationsResponse" /> ready to return to the caller</returns>
    public static GetAllSiteConfigurationsResponse ToDto(this SiteConfiguration siteConfiguration)
        => new(siteConfiguration);
}
