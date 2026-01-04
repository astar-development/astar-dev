using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Technical.Debt.Reporting;

namespace AStar.Dev.Files.Api.Duplicates.Get.V1;

[Refactor(2, 4, "Unless I am mistaken, we're returning the Domain Model...")]
public sealed class GetDuplicatesQueryResponse
{
    private readonly List<DuplicatesDetails> duplicates = [];

    public GetDuplicatesQueryResponse(IGrouping<FileGrouping, DuplicatesDetails> duplicatesGroupings)
    {
        FileGrouping = duplicatesGroupings.Key;

        foreach(DuplicatesDetails duplicatesDetails in duplicatesGroupings) duplicates.Add(duplicatesDetails);
    }

    public FileGrouping FileGrouping { get; private set; }

    public IEnumerable<DuplicatesDetails> Duplicates => duplicates;
}
