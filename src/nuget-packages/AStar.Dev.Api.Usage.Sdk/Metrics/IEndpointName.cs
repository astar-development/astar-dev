namespace AStar.Dev.Api.Usage.Sdk.Metrics;

/// <summary>
///     The <see cref="IEndpointName" /> interface contains additional information for logging the API Usage.
/// </summary>
public interface IEndpointName
{
    /// <summary>
    ///     Gets the name of the Endpoint.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the HTTP Method type of the Endpoint.
    /// </summary>
    string HttpMethod { get; }
}
