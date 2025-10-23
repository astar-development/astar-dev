using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Files.Classifications.Api.Services;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Service to fetch file classifications from the database.
/// </summary>
public class FileClassificationsService : IFileClassificationsService
{
    private readonly FilesContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileClassificationsService"/> class.
    /// </summary>
    /// <param name="context">The database context for accessing file classifications.</param>
    public FileClassificationsService(FilesContext context) => _context = context;

    /// <summary>
    /// Retrieves a list of distinct file classifications.
    /// </summary>
    /// <returns>A collection of file classification names.</returns>
    public async Task<IEnumerable<string>> GetFileClassificationsAsync() => await _context.FileClassifications.Select(fc => fc.Name).Distinct().ToListAsync();
}