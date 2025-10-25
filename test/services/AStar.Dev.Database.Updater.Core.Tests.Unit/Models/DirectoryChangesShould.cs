using AStar.Dev.Database.Updater.Core.Models;

namespace AStar.Dev.Database.Updater.Core.Tests.Unit.Models;

public sealed class DirectoryChangesShould
{
    [Fact]
    public void ReturnTheExpectedToString() => new DirectoryChanges().ToString().ShouldMatchApproved();
}
