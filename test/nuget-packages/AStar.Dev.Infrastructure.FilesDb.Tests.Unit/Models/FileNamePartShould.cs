using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

public class FileNamePartShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new FileNamePart { FileClassifications = new List<FileClassification> { new()  { Id = 1 } } }
           .ToJson()
           .ShouldMatchApproved();
}
