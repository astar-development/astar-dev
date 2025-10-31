namespace AStar.Dev.Functional.Extensions.Tests.Unit;

public class TryTests
{
    [Fact]
    public void RunShouldReturnOkResultWhenFunctionSucceeds()
    {
        const int expectedValue = 42;

        var result = Try.Run(SuccessFunc);

        _ = result.ShouldBeOfType<Result<int, Exception>.Ok>();
        Pattern.IsOk(result).ShouldBeTrue();
        Pattern.IsError(result).ShouldBeFalse();
        ((Result<int, Exception>.Ok)result).Value.ShouldBe(expectedValue);

        return;

        int SuccessFunc() => expectedValue;
    }

    [Fact]
    public void RunShouldReturnErrorResultWhenFunctionThrows()
    {
        const string exceptionMessage  = "Test exception";
        Exception    expectedException = new InvalidOperationException(exceptionMessage);

        var result = Try.Run(FailingFunc);

        _ = result.ShouldBeOfType<Result<int, Exception>.Error>();
        Pattern.IsOk(result).ShouldBeFalse();
        Pattern.IsError(result).ShouldBeTrue();
        ((Result<int, Exception>.Error)result).Reason.ShouldBe(expectedException);
        ((Result<int, Exception>.Error)result).Reason.Message.ShouldBe(exceptionMessage);

        return;

        int FailingFunc() => throw expectedException;
    }

    [Fact]
    public void RunShouldCaptureSpecificExceptionTypes()
    {
        var result = Try.Run(ArgNullFunc);

        var error = ((Result<int, Exception>.Error)result).Reason;
        _ = error.ShouldBeOfType<ArgumentNullException>();
        (error as ArgumentNullException)?.ParamName.ShouldBe("testParam");

        return;

        static int ArgNullFunc() => throw new ArgumentNullException("testParam");
    }

    [Fact]
    public async Task RunAsyncShouldReturnOkResultWhenAsyncFunctionSucceedsAsync()
    {
        const string expectedValue = "async result";

        var result = await Try.RunAsync(SuccessFunc);

        _ = result.ShouldBeOfType<Result<string, Exception>.Ok>();
        Pattern.IsOk(result).ShouldBeTrue();
        Pattern.IsError(result).ShouldBeFalse();
        ((Result<string, Exception>.Ok)result).Value.ShouldBe(expectedValue);

        return;

        Task<string> SuccessFunc() => Task.FromResult(expectedValue);
    }

    [Fact]
    public async Task RunAsyncShouldReturnErrorResultWhenAsyncFunctionThrowsAsync()
    {
        const string exceptionMessage  = "Async test exception";
        Exception    expectedException = new InvalidOperationException(exceptionMessage);

        var result = await Try.RunAsync(FailingFunc);

        _ = result.ShouldBeOfType<Result<string, Exception>.Error>();
        Pattern.IsOk(result).ShouldBeFalse();
        Pattern.IsError(result).ShouldBeTrue();
        ((Result<string, Exception>.Error)result).Reason.ShouldBe(expectedException);
        ((Result<string, Exception>.Error)result).Reason.Message.ShouldBe(exceptionMessage);

        return;

        Task<string> FailingFunc() => Task.FromException<string>(expectedException);
    }

    [Fact]
    public async Task RunAsyncShouldCaptureExceptionFromAsyncAwaitOperationAsync()
    {
        var result = await Try.RunAsync(FailingAsyncFunc);

        Pattern.IsError(result).ShouldBeTrue();
        _ = ((Result<int, Exception>.Error)result).Reason.ShouldBeOfType<TimeoutException>();
        ((Result<int, Exception>.Error)result).Reason.Message.ShouldBe("Operation timed out");

        return;

        static async Task<int> FailingAsyncFunc()
        {
            await Task.Delay(1);

            throw new TimeoutException("Operation timed out");
        }
    }

    [Fact]
    public void RunShouldWorkWithLambdaExpressions()
    {
        var result = Try.Run(() => 5 + 5);

        Pattern.IsOk(result).ShouldBeTrue();
        ((Result<int, Exception>.Ok)result).Value.ShouldBe(10);
    }

    [Fact]
    public async Task RunAsyncShouldWorkWithAsyncLambdaExpressionsAsync()
    {
        var result = await Try.RunAsync(async () =>
        {
            await Task.Delay(1);

            return "completed";
        });

        Pattern.IsOk(result).ShouldBeTrue();
        ((Result<string, Exception>.Ok)result).Value.ShouldBe("completed");
    }

    [Fact]
    public void RunShouldPreserveOriginalExceptionWithoutWrapping()
    {
        var customException = new CustomTestException("Custom exception test");

        var result = Try.Run<int>(() => throw customException);

        var error = ((Result<int, Exception>.Error)result).Reason;
        _ = error.ShouldBeOfType<CustomTestException>();
        error.ShouldBeSameAs(customException);
    }

    [Fact]
    public async Task RunAsyncShouldPreserveOriginalExceptionWithoutWrappingAsync()
    {
        var customException = new CustomTestException("Async custom exception test");

        var result = await Try.RunAsync<string>(async () =>
        {
            await Task.Delay(1);

            throw customException;
        });

        var error = ((Result<string, Exception>.Error)result).Reason;
        _ = error.ShouldBeOfType<CustomTestException>();
        error.ShouldBeSameAs(customException);
    }

    [Fact]
    public void TryRunCapturesSuccess()
    {
        var result = Try.Run(() => 42);

        var output = result.Match(
            ok => ok,
            ex => -1);

        Assert.Equal(42, output);
    }

    [Fact]
    public void TryRunCapturesException()
    {
        var result = Try.Run<int>(() => throw new InvalidOperationException("fail"));

        var output = result.Match(
            ok => ok,
            ex => -1);

        Assert.Equal(-1, output);
    }

    [Fact]
    public void TryMatchReturnsCorrectBranch()
    {
        var success = Try.Run(() => "done");
        var failure = Try.Run<string>(() => throw new InvalidOperationException("fail"));

        var a = success.Match(x => $"OK: {x}", ex => $"ERR: {ex.Message}");
        var b = failure.Match(x => $"OK: {x}", ex => $"ERR: {ex.Message}");

        Assert.Equal("OK: done", a);
        Assert.Equal("ERR: fail", b);
    }
}
