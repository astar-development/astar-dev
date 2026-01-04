using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Technical.Debt.Reporting;

namespace AStar.Dev.Files.Api.AllFiles.V1;

[Refactor(2, 4, "Unless I am mistaken, we're returning the Domain Model...")]
public sealed class GetAllFilesQueryResponse(FileDetail scrapeDirectory)
{
    /// <summary>
    /// </summary>
    public FileDetail FileDetail { get; set; } = scrapeDirectory;
}
