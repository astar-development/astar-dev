using System.ComponentModel.DataAnnotations;
using AStar.Dev.Api.Client.Sdk.Shared;

namespace AStar.Dev.Files.Api.Client.SDK.FilesApi;

/// <summary>
///     The <see href="FilesApiConfiguration"></see> class containing the current configuration settings
/// </summary>
public sealed class FilesApiConfiguration : IApiConfiguration
{
    /// <summary>
    ///     Gets the Section Location for the API configuration from within the appSettings.Json file
    /// </summary>
    public const string SectionLocation = "ApiConfiguration:FilesApiConfiguration";

    /// <inheritdoc />
    [Required]
    public Uri BaseUrl { get; set; } = new("https://not.set.com");

    /// <inheritdoc />
    [Required]
    public required string[] Scopes { get; set; }
}
