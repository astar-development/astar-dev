using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Classifications.Api.Services;

/// <summary>
/// Service to fetch file classifications from the database.
/// </summary>
public class FileClassificationsService2 : IFileClassificationsService2
{
    private readonly FilesContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileClassificationsService2"/> class.
    /// </summary>
    /// <param name="context">The database context for accessing file classifications.</param>
    public FileClassificationsService2(FilesContext context) => _context = context;

    /// <summary>
    /// Retrieves a list of distinct file classifications.
    /// </summary>
    /// <returns>A collection of file classification names.</returns>
    public async Task<IList<FileClassification>> GetFileClassificationsAsync() => await _context.FileClassifications.Select(fc => new FileClassification { Id = fc.Id, Name = fc.Name, IncludeInSearch = fc.IncludeInSearch, Celebrity = fc.Celebrity, SearchLevel = fc.SearchLevel, ParentId = fc.ParentId }).ToListAsync();
}
