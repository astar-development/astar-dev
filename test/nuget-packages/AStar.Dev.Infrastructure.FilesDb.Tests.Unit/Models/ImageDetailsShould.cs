using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

[TestSubject(typeof(ImageDetails))]
public class ImageDetailsShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new ImageDetails { Id = 1, FileDetailsId = 2, Width = 123, Height = 456 }.ToJson().ShouldMatchApproved();

    [Fact]
    public void ReturnTheExpectedToString()
        => new ImageDetails { Id = 1, FileDetailsId = 2, Width = 123, Height = 456 }.ToString().ShouldMatchApproved();
}
