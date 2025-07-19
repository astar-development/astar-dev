namespace AStar.Dev.Functional.Extensions.Tests.Unit;

public class ResultShould
{
    private static Result<int, string>.Ok Ok(int value) => new(value);

    private static Result<int, string>.Error Err(string error) => new(error);

    [Fact]
    public void Match_ReturnsCorrectBranch()
    {
        var ok  = Ok(42);
        var err = Err("fail");

        var okResult = ok.Match(
                                success => $"Success: {success}",
                                error => $"Error: {error}");

        var errResult = err.Match(
                                  success => $"Success: {success}",
                                  error => $"Error: {error}");

        Assert.Equal("Success: 42", okResult);
        Assert.Equal("Error: fail", errResult);
    }

    [Fact]
    public void Map_TransformsSuccess()
    {
        var result = Ok(10).Map(x => x * 2);

        var output = result.Match(
                                  success => success,
                                  error => -1);

        Assert.Equal(20, output);
    }

    [Fact]
    public void Map_PreservesError()
    {
        var result = Err("oops").Map(x => x * 2);

        var output = result.Match(
                                  success => success,
                                  error => -1);

        Assert.Equal(-1, output);
    }

    [Fact]
    public void Bind_ChainsSuccess()
    {
        Result<int, string> DoubleIfEven(int x)
        {
            return x % 2 == 0 ? Ok(x * 2) : Err("odd");
        }

        var result = Ok(4).Bind(DoubleIfEven);

        var output = result.Match(
                                  success => success,
                                  error => -1);

        Assert.Equal(8, output);
    }

    [Fact]
    public void Bind_ShortCircuitsOnError()
    {
        var result = Err("fail").Bind(x => Ok(x * 2));

        var output = result.Match(
                                  success => success,
                                  error => -1);

        Assert.Equal(-1, output);
    }

    [Fact]
    public void Tap_InvokesSideEffectOnSuccess()
    {
        var tapped = 0;
        var result = Ok(5).Tap(x => tapped = x);
        Assert.Equal(5, tapped);
    }

    [Fact]
    public void Tap_DoesNotInvokeOnError()
    {
        var tapped = 0;
        var result = Err("fail").Tap(x => tapped = x);
        Assert.Equal(0, tapped);
    }

    [Fact]
    public void Linq_Query_ComposesCorrectly()
    {
        var result = from a in Ok(2)
                     from b in Ok(3)
                     select a + b;

        var output = result.Match(
                                  success => success,
                                  error => -1);

        Assert.Equal(5, output);
    }

    [Fact]
    public async Task MapAsync_TransformsSuccess()
    {
        var result = await new ValueTask<Result<int, string>>(Ok(3))
                         .MapAsync(x => x + 1);

        var output = result.Match(
                                  success => success,
                                  error => -1);

        Assert.Equal(4, output);
    }

    [Fact]
    public async Task BindAsync_ChainsAsyncResults()
    {
        ValueTask<Result<int, string>> DoubleAsync(int x)
        {
            return new(new Result<int, string>.Ok(x * 2));
        }

        var result = await new ValueTask<Result<int, string>>(Ok(5))
                         .BindAsync(DoubleAsync);

        var output = result.Match(
                                  success => success,
                                  error => -1);

        Assert.Equal(10, output);
    }

    [Fact]
    public async Task MatchAsync_HandlesBothBranches()
    {
        var ok = await new ValueTask<Result<int, string>>(Ok(7))
                     .MatchAsync(
                                 success => $"Yay: {success}",
                                 error => $"Oops: {error}");

        var err = await new ValueTask<Result<int, string>>(Err("bad"))
                      .MatchAsync(
                                  success => $"Yay: {success}",
                                  error => $"Oops: {error}");

        Assert.Equal("Yay: 7",    ok);
        Assert.Equal("Oops: bad", err);
    }
}
