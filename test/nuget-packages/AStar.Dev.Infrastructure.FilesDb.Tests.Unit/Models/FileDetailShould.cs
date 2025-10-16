namespace AStar.Dev.Infrastructure.FilesDb.Models;

public sealed class FileDetailShould
{
    [Fact]
    public void ReturnTheExpectedToStringRepresentation()
    {
        var fileDetail = new FileDetail { FileName = new("test.txt"), DirectoryName = new("C:\\") };

        fileDetail.ToString().ShouldMatchApproved();
    }
}
