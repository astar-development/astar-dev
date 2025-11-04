using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Models;

public sealed class TagToIgnoreShould
{
    [Fact]
    public void ReturnTheExpectedToStringOutput()
        => new TagToIgnore { UpdatedOn = new DateTimeOffset(2025, 1, 2, 3, 4, 5, new TimeSpan(0, 0, 0)) }.ToString().ShouldMatchApproved();
}
