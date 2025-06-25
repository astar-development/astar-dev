using System.ComponentModel.DataAnnotations;
using AStar.Dev.Api.Client.Sdk.Shared;

namespace AStar.Dev.Images.Api.Client.SDK.ImagesApi;

/// <summary>
///     The <see href="ImagesApiConfiguration"></see> class containing the current configuration settings
/// </summary>
public sealed class ImagesApiConfiguration : IApiConfiguration
{
    /// <summary>
    ///     Gets the Section Location for the API configuration from within the appSettings.Json file
    /// </summary>
    public const string SectionLocation = "ApiConfiguration:ImagesApiConfiguration";

    /// <inheritdoc />
    [Required]
    public Uri BaseUrl { get; set; } = new("https://not.set.com");

    /// <inheritdoc />
    [Required]
    public required string[] Scopes { get; set; }
}
