using AStar.Dev.Functional.Extensions;
using AStar.Dev.Source.Generators.Attributes;
using AStar.Dev.Web.Models;

namespace AStar.Dev.Web.Services;

[Service]
public sealed class FileClassificationsService : IFileClassificationsService
{
    /// <inheritdoc/>
    public async Task<Result<IList<FileClassification>, ErrorResponse>> GetFileClassificationsAsync()
        => new Result<IList<FileClassification>, ErrorResponse>.Ok(await Task.FromResult(Enumerable.Empty<FileClassification>().ToList()));
}
