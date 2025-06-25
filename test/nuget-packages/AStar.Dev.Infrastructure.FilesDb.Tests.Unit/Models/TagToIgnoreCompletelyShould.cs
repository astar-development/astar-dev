namespace AStar.Dev.Infrastructure.FilesDb.Models;

public sealed class TagToIgnoreCompletelyShould
{
    [Fact]
    public void ReturnTheExpectedToStringOutput() =>
        new ModelToIgnore().ToString().ShouldMatchApproved();
}
