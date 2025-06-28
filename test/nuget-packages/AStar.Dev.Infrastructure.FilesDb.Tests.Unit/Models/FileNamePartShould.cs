using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

[TestSubject(typeof(FileNamePart))]
public class FileNamePartShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new FileNamePart { Id = 1, Text = "Mock Text", FileClassifications = new List<FileClassification> { new () { Id = 1, Celebrity = true, Name = "Mock Classification" } } }.ToJson()
                                                                                                                                                                                    .ShouldMatchApproved();
}
