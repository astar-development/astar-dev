using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

[TestSubject(typeof(FileHandle))]
public class FileHandleShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new FileHandle { Value = "some-handle" }
           .ToJson()
           .ShouldMatchApproved();

    [Fact]
    public void ContainTheExpectedCreateMethod()
        => FileHandle.Create("some-handle")
                     .ToJson()
                     .ShouldMatchApproved();

    [Fact]
    public void ContainTheExpectedImplicitConversionToString()
    {
        string sut = FileHandle.Create("some-handle");

        sut.ShouldBe("some-handle");
    }
}
