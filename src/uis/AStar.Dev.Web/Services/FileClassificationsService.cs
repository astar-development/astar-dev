using AStar.Dev.Annotations;
using AStar.Dev.Web.Models;

namespace AStar.Dev.Web.Services;

[RegisterService]
public sealed class FileClassificationsService(IFileClassificationsService fileClassificationsService) : IFileClassificationsService
{
    /// <inheritdoc/>
    public async Task<IList<FileClassification>> GetFileClassificationsAsync()
        => (await fileClassificationsService.GetFileClassificationsAsync())
        .Select(fc => new FileClassification(fc))
        .ToList();
}
