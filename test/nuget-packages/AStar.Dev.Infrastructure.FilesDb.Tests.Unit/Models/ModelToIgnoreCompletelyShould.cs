using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Models;

public sealed class ModelToIgnoreCompletelyShould
{
    [Fact]
    public void ReturnTheExpectedToStringOutput()
        => new ModelToIgnore { UpdatedOn = new DateTimeOffset(2025, 1, 2, 3, 4, 5, new TimeSpan(0, 0, 0)) }.ToString().ShouldMatchApproved();
}
