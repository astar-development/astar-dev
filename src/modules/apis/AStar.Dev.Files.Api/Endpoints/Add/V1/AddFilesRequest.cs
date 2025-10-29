using AStar.Dev.Api.Usage.Sdk.Metrics;
using Microsoft.AspNetCore.Http;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

/// <summary>
///     The <see cref="AddFilesRequest" /> contains the detail of the files being added.
/// </summary>
public class AddFilesRequest : IEndpointName
{
    /// <summary>
    /// </summary>
    public required IReadOnlyCollection<FileDetailToAdd> FilesToAdd { get; set; } = [];

    /// <inheritdoc />
    public string Name => EndpointConstants.AddFilesGroupName;

    /// <inheritdoc />
    public string HttpMethod => HttpMethods.Post;
}
