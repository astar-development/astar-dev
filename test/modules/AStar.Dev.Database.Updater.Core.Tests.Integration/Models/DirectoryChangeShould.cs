using AStar.Dev.Database.Updater.Core.Models;

namespace AStar.Dev.Database.Updater.Core.Tests.Integration.Models;

public sealed class DirectoryChangeShould
{
    [Fact]
    public void ReturnTheExpectedToString() => new DirectoryChange("", "").ToString().ShouldMatchApproved();
}

