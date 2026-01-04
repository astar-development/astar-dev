namespace AStar.Dev.Api.Client.Sdk.Shared;

/// <summary>
///     Defines the <see cref="IApiConfiguration" /> interface
/// </summary>
public interface IApiConfiguration
{
    /// <summary>
    ///     Gets or sets the Base URL for the API
    /// </summary>
    Uri BaseUrl { get; set; }

    /// <summary>
    ///     Gets or sets the Scopes for the API
    /// </summary>
    string[] Scopes { get; set; }
}
