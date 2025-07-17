using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

[TestSubject(typeof(FileClassification))]
public class FileClassificationShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new FileClassification
           {
               Id        = 1,
               Celebrity = true,
               Name      = "Mock Name",
               FileDetails   = new List<FileDetail>
                               {
                                   new() { Id    = 1, DirectoryName = "Mock Directory", FileName = "Mock File Name", UpdatedOn = new (new (2025, 6, 28, 22, 20, 37, DateTimeKind.Utc)) }
                               },
               FileNameParts = new List<FileNamePart> { new()  { Id = 1, Text          = "Mock Text Part" } }
           }.ToJson().ShouldMatchApproved();
}
