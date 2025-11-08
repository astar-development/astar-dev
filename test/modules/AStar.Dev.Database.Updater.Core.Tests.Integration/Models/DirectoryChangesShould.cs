using AStar.Dev.Database.Updater.Core.Models;

namespace AStar.Dev.Database.Updater.Core.Tests.Integration.Models;

public sealed class DirectoryChangesShould
{
    [Fact]
    public void ReturnTheExpectedToString() => new DirectoryChanges().ToString().ShouldMatchApproved();
}
