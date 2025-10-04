using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Database.Updater.Api.FileKeywordProcessor;

public class EfKeywordProvider(FilesContext db) : IKeywordProvider
{
    public async Task<IReadOnlyList<string>> GetKeywordsAsync(CancellationToken cancellationToken = default) => await db.FileClassifications
                                                                                                                        .Select(k => k.Name)
                                                                                                                        .ToListAsync(cancellationToken);
}
