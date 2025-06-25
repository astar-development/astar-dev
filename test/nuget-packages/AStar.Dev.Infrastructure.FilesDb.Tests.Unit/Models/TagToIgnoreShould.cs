namespace AStar.Dev.Infrastructure.FilesDb.Models;

public sealed class TagToIgnoreShould
{
    [Fact]
    public void ReturnTheExpectedToStringOutput() =>
        new TagToIgnore().ToString().ShouldMatchApproved();
}
