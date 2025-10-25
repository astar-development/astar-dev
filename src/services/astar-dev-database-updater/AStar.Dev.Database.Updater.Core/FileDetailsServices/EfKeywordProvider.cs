using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Database.Updater.Core.FileDetailsServices;

/// <summary>
/// </summary>
/// <param name="db"></param>
public class EfKeywordProvider(FilesContext db) : IKeywordProvider
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<FileNamePartsWithClassifications>> GetKeywordsAsync(CancellationToken cancellationToken = default)
        => await db.FileClassifications
                   .Include(d => d.FileNameParts)
                   .SelectMany(f => f.FileNameParts.Select(fp => new FileNamePartsWithClassifications
                   {
                       Id = fp.Id,
                       Text = fp.Text,
                       Name = f.Name,
                       Celebrity = f.Celebrity,
                       IncludeInSearch = f.IncludeInSearch
                   }))
                   .ToListAsync(cancellationToken);
}
