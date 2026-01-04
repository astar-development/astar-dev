namespace AStar.Dev.Infrastructure.FilesDb.Models;

public sealed class FileDetailShould
{
    [Fact]
    public void ReturnTheExpectedToStringRepresentation()
    {
        var fileDetail = new FileDetail { FileName = new FileName("test.txt"), DirectoryName = new DirectoryName("C:\\"), };

        fileDetail.ToString().ShouldMatchApproved();
    }
}
