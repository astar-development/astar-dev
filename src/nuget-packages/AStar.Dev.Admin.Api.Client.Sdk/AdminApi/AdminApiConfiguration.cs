using System.ComponentModel.DataAnnotations;
using AStar.Dev.Api.Client.Sdk.Shared;

namespace AStar.Dev.Admin.Api.Client.Sdk.AdminApi;

/// <summary>
///     The <see href="AdminApiConfiguration"></see> class containing the current configuration settings.
/// </summary>
public sealed class AdminApiConfiguration : IApiConfiguration
{
    /// <summary>
    ///     Gets the Section Location for the API configuration from within the appSettings.Json file.
    /// </summary>
    public const string SectionLocation = "apiConfiguration:adminApiConfiguration";

    /// <inheritdoc />
    [Required]
    public Uri BaseUrl { get; set; } = new("https://not.set.com");

    /// <inheritdoc />
    [Required]
    public required string[] Scopes { get; set; }
}
