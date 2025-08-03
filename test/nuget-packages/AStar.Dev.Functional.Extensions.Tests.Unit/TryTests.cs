namespace AStar.Dev.Functional.Extensions.Tests.Unit;

public class TryTests
{
    [Fact]
    public void Try_Run_CapturesSuccess()
    {
        var result = Try<int>.Run(() => 42);

        var output = result.Match(
                                  ok => ok,
                                  ex => -1);

        Assert.Equal(42, output);
    }

    [Fact]
    public void Try_Run_CapturesException()
    {
        var result = Try<int>.Run(() => throw new InvalidOperationException("fail"));

        var output = result.Match(
                                  ok => ok,
                                  ex => -1);

        Assert.Equal(-1, output);
    }

    [Fact]
    public void Try_Match_ReturnsCorrectBranch()
    {
        var success = Try<string>.Run(() => "done");
        var failure = Try<string>.Run(() => throw new ("fail"));

        var a = success.Match(x => $"OK: {x}", ex => $"ERR: {ex.Message}");
        var b = failure.Match(x => $"OK: {x}", ex => $"ERR: {ex.Message}");

        Assert.Equal("OK: done", a);
        Assert.StartsWith("ERR: fail", b);
    }
}
