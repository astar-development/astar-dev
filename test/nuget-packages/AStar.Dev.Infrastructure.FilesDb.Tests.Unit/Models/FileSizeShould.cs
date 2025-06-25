namespace AStar.Dev.Infrastructure.FilesDb.Models;

public sealed class FileSizeShould
{
    [Fact]
    public void ReturnTheExpectedToStringOutput() =>
        FileSize.Create(1, 2, 3).ToString().ShouldMatchApproved();
}
