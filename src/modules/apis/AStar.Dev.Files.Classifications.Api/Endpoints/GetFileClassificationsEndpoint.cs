using AStar.Dev.Files.Classifications.Api.Services;
using FastEndpoints;

namespace AStar.Dev.Files.Classifications.Api.Endpoints;

/// <summary>
/// Endpoint to retrieve file classifications.
/// </summary>
public class GetFileClassificationsEndpoint(IFileClassificationsService service) : EndpointWithoutRequest<IEnumerable<string>>
{
    /// <inheritdoc/>
    public override void Configure()
    {
        Get("/file-classifications");
        AllowAnonymous();
    }

    /// <inheritdoc/>
    public override async Task HandleAsync(CancellationToken ct)
    {
        IEnumerable<string> classifications = await service.GetFileClassificationsAsync();
        _ = await Send.OkAsync(classifications, ct);
    }
}
