using System.Net;
using JetBrains.Annotations;

namespace AStar.Dev.Test.Helpers.Minimal.Api;

[TestSubject(typeof(ResultExtensions))]

// ReSharper disable once InconsistentNaming
public sealed class IResultExtensionsShould
{
    private static readonly string[] Expected = ["a", "b", "c"];

    [Fact]
    public void ReturnTheExpectedValueFromTheGetResultValueMethodWhenCalledOnGetEndpoint()
    {
        IResult sut = TestEndpoints.GetAll();

        var result = sut.ResultValue<object>();

        result.ShouldBeEquivalentTo(Expected);
    }

    [Fact]
    public void ReturnTheExpectedValueFromTheGetResultStatusCodeMethodWhenCalledOnGetEndpoint()
    {
        IResult sut = TestEndpoints.GetAll();

        var result = sut.ResultStatusCode();

        result.ShouldBe((int)HttpStatusCode.OK);
    }

    [Fact]
    public void ReturnTheExpectedValueFromTheGetResultValueMethodWhenCalledOnCreateEndpoint()
    {
        IResult sut = TestEndpoints.Create();

        var result = sut.ResultValue<object>();

        result.ShouldBeEquivalentTo(new());
    }

    [Fact]
    public void ReturnTheExpectedValueFromTheGetResultStatusCodeMethodWhenCalledOnCreateEndpoint()
    {
        IResult sut = TestEndpoints.Create();

        var result = sut.ResultStatusCode();

        result.ShouldBe((int)HttpStatusCode.Created);
    }
}
