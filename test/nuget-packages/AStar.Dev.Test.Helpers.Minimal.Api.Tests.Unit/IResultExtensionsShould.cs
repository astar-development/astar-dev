using System.Net;

namespace AStar.Dev.Test.Helpers.Minimal.Api;

[TestSubject(typeof(ResultExtensions))]

// ReSharper disable once InconsistentNaming
public sealed class IResultExtensionsShould
{
    private static readonly string[] Expected = ["a", "b", "c",];

    [Fact]
    public void ReturnTheExpectedValueFromTheGetResultValueMethodWhenCalledOnGetEndpoint()
    {
        IResult sut = TestEndpoints.GetAll();

        var result = sut.GetResultValue<object>();

        result.ShouldBeEquivalentTo(Expected);
    }

    [Fact]
    public void ReturnTheExpectedValueFromTheGetResultStatusCodeMethodWhenCalledOnGetEndpoint()
    {
        IResult sut = TestEndpoints.GetAll();

        var result = sut.GetResultStatusCode();

        result.ShouldBe((int)HttpStatusCode.OK);
    }

    [Fact]
    public void ReturnTheExpectedValueFromTheGetResultValueMethodWhenCalledOnCreateEndpoint()
    {
        IResult sut = TestEndpoints.Create();

        var result = sut.GetResultValue<object>();

        result.ShouldBeEquivalentTo(new object());
    }

    [Fact]
    public void ReturnTheExpectedValueFromTheGetResultStatusCodeMethodWhenCalledOnCreateEndpoint()
    {
        IResult sut = TestEndpoints.Create();

        var result = sut.GetResultStatusCode();

        result.ShouldBe((int)HttpStatusCode.Created);
    }
}
