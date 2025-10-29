using AStar.Dev.Database.Updater.Core.Models;

namespace AStar.Dev.Database.Updater.Core.Tests.Unit.Models;

public sealed class DirectoryChangeShould
{
    [Fact]
    public void ReturnTheExpectedToString() => new DirectoryChange("", "").ToString().ShouldMatchApproved();
}

