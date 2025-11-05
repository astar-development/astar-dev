using AStar.Dev.Api.Usage.Sdk.Metrics;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

/// <summary>
///     Represents a request to retrieve file classifications based on pagination parameters.
///     The object supports HTTP GET operation to access the required endpoint.
/// </summary>
[UsedImplicitly]
public record GetFileClassificationRequest(int CurrentPage = 1, int ItemsPerPage = 10) : IEndpointName,IPagingParameters
{
    /// <inheritdoc />
    public string Name => EndpointConstants.GetFileClassificationsGroupName;

    /// <inheritdoc />
    public string HttpMethod => HttpMethods.Get;
}
