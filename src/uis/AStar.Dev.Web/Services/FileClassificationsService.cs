using AStar.Dev.Web.Models;

namespace AStar.Dev.Web.Services;

public class FileClassificationsService(AStar.Dev.Files.Classifications.Api.Services.IFileClassificationsService2 fileClassificationsService) : AStar.Dev.Web.Services.IFileClassificationsService
{
    /// <inheritdoc/>
    public async Task<IList<FileClassification>> GetFileClassificationsAsync()
        => (await fileClassificationsService.GetFileClassificationsAsync())
        .Select(fc => new FileClassification(fc))
        .ToList();
}
