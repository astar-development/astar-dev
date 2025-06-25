namespace AStar.Dev.Infrastructure.FilesDb.Models;

public sealed class FileDetailShould
{
    [Fact]
    public void ReturnTheExpectedToStringRepresentation()
    {
        var fileDetail = new FileDetail();

        fileDetail.ToString().ShouldMatchApproved();
    }
}
