using AStar.Dev.Files.Classifications.Api.Services;
using AStar.Dev.Web.Models;

namespace AStar.Dev.Web.Services;

public class FileClassificationsService(IFileClassificationsService2 fileClassificationsService) : IFileClassificationsService
{
    /// <inheritdoc/>
    public async Task<IList<FileClassification>> GetFileClassificationsAsync()
        => (await fileClassificationsService.GetFileClassificationsAsync())
        .Select(fc => new FileClassification(fc))
        .ToList();
}
