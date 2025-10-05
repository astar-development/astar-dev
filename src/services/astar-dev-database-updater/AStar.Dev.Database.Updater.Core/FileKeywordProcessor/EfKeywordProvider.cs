using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
/// <param name="db"></param>
public class EfKeywordProvider(FilesContext db) : IKeywordProvider
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetKeywordsAsync(CancellationToken cancellationToken = default) => await db.FileClassifications
                                                                                                                        .Select(k => k.Name)
                                                                                                                        .ToListAsync(cancellationToken);
}
