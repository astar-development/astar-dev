namespace AStar.Dev.Functional.Extensions.Tests.Unit;

public class TryTests
{
    [Fact]
    public void RunShouldReturnOkResultWhenFunctionSucceeds()
    {
        var expectedValue = 42;

        int successFunc()
        {
            return expectedValue;
        }

        var result = Try.Run(successFunc);

        _ = result.ShouldBeOfType<Result<int, Exception>.Ok>();
        Pattern.IsOk(result).ShouldBeTrue();
        Pattern.IsError(result).ShouldBeFalse();
        ((Result<int, Exception>.Ok)result).Value.ShouldBe(expectedValue);
    }

    [Fact]
    public void RunShouldReturnErrorResultWhenFunctionThrows()
    {
        var exceptionMessage = "Test exception";
        Exception expectedException = new InvalidOperationException(exceptionMessage);

        int failingFunc()
        {
            throw expectedException;
        }

        var result = Try.Run(failingFunc);

        _ = result.ShouldBeOfType<Result<int, Exception>.Error>();
        Pattern.IsOk(result).ShouldBeFalse();
        Pattern.IsError(result).ShouldBeTrue();
        ((Result<int, Exception>.Error)result).Reason.ShouldBe(expectedException);
        ((Result<int, Exception>.Error)result).Reason.Message.ShouldBe(exceptionMessage);
    }

    [Fact]
    public void RunShouldCaptureSpecificExceptionTypes()
    {
        static int argNullFunc()
        {
            throw new ArgumentNullException("testParam");
        }

        var result = Try.Run(argNullFunc);

        var error = ((Result<int, Exception>.Error)result).Reason;
        _ = error.ShouldBeOfType<ArgumentNullException>();
        (error as ArgumentNullException)?.ParamName.ShouldBe("testParam");
    }

    [Fact]
    public async Task RunAsyncShouldReturnOkResultWhenAsyncFunctionSucceedsAsync()
    {
        var expectedValue = "async result";

        Task<string> successFunc()
        {
            return Task.FromResult(expectedValue);
        }

        var result = await Try.RunAsync(successFunc);

        _ = result.ShouldBeOfType<Result<string, Exception>.Ok>();
        Pattern.IsOk(result).ShouldBeTrue();
        Pattern.IsError(result).ShouldBeFalse();
        ((Result<string, Exception>.Ok)result).Value.ShouldBe(expectedValue);
    }

    [Fact]
    public async Task RunAsyncShouldReturnErrorResultWhenAsyncFunctionThrowsAsync()
    {
        var exceptionMessage = "Async test exception";
        Exception expectedException = new InvalidOperationException(exceptionMessage);

        Task<string> failingFunc()
        {
            return Task.FromException<string>(expectedException);
        }

        var result = await Try.RunAsync(failingFunc);

        _ = result.ShouldBeOfType<Result<string, Exception>.Error>();
        Pattern.IsOk(result).ShouldBeFalse();
        Pattern.IsError(result).ShouldBeTrue();
        ((Result<string, Exception>.Error)result).Reason.ShouldBe(expectedException);
        ((Result<string, Exception>.Error)result).Reason.Message.ShouldBe(exceptionMessage);
    }

    [Fact]
    public async Task RunAsyncShouldCaptureExceptionFromAsyncAwaitOperationAsync()
    {
        static async Task<int> failingAsyncFunc()
        {
            await Task.Delay(1);

            throw new TimeoutException("Operation timed out");
        }

        var result = await Try.RunAsync(failingAsyncFunc);

        Pattern.IsError(result).ShouldBeTrue();
        _ = ((Result<int, Exception>.Error)result).Reason.ShouldBeOfType<TimeoutException>();
        ((Result<int, Exception>.Error)result).Reason.Message.ShouldBe("Operation timed out");
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
