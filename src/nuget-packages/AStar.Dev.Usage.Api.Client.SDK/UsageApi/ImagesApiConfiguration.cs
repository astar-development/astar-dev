using System.ComponentModel.DataAnnotations;
using AStar.Dev.Api.Client.Sdk.Shared;

namespace AStar.Dev.Usage.Api.Client.SDK.UsageApi;

/// <summary>
///     The <see href="UsageApiConfiguration"></see> class containing the current configuration settings
/// </summary>
public sealed class UsageApiConfiguration : IApiConfiguration
{
    /// <summary>
    ///     Gets the Section Location for the API configuration from within the appSettings.Json file
    /// </summary>
    public const string SectionLocation = "ApiConfiguration:UsageApiConfiguration";

    /// <inheritdoc />
    [Required]
    public Uri BaseUrl { get; set; } = new("https://not.set.com");

    /// <inheritdoc />
    [Required]
    public required string[] Scopes { get; set; }
}
