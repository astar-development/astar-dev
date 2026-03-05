using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Files.Api.MarkForMoving.V1;

/// <summary>
/// </summary>
public sealed record UnMarkFileForMovingCommand(int FileId) : IEndpointName
{
    /// <inheritdoc />
    [JsonIgnore]
    public string Name => EndpointConstants.AllFilesEndpoint;

    /// <inheritdoc />
    [JsonIgnore]
    public string HttpMethod => HttpMethods.Get;
}
