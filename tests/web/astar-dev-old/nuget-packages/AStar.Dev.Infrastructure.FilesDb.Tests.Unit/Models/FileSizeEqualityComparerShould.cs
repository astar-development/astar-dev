namespace AStar.Dev.Infrastructure.FilesDb.Models;

public sealed class FileSizeEqualityComparerShould
{
    [Fact]
    public void ReturnTrueWhenTheTwoInstancesOfFileSizeAreEquivalent()
    {
        var fileSize1 = FileSize.Create(1, 2, 3);
        var fileSize2 = FileSize.Create(1, 2, 3);
        var comparer  = new FileSizeEqualityComparer();

        comparer.Equals(fileSize1, fileSize2).ShouldBeTrue();
    }

    [Fact]
    public void ReturnFalseWhenTheTwoInstancesOfFileSizeAreNotEquivalent()
    {
        var fileSize1 = FileSize.Create(1, 2, 3);
        var fileSize2 = FileSize.Create(1, 2, 4);
        var comparer  = new FileSizeEqualityComparer();

        comparer.Equals(fileSize1, fileSize2).ShouldBeFalse();
    }
}
