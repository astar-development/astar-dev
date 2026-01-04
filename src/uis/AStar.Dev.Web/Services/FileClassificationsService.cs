using AStar.Dev.Web.Models;

namespace AStar.Dev.Web.Services;

//[Service]
public sealed class FileClassificationsService : IFileClassificationsService
{
    /// <inheritdoc/>
    public async Task<IList<FileClassification>> GetFileClassificationsAsync()
        => Enumerable.Empty<FileClassification>().ToList();
}
