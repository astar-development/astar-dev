using AStar.Dev.Functional.Extensions;

namespace AStar.Dev.Database.Updater.Core;

public class TryTests
{
    [Fact]
    public void RunShouldReturnOkResultWhenFunctionSucceeds()
    {
        var expectedValue = 42;
        var successFunc   = () => expectedValue;

        var result = Try.Run(successFunc);

        result.ShouldBeOfType<Result<int, Exception>.Ok>();
        Pattern.IsOk(result).ShouldBeTrue();
        Pattern.IsError(result).ShouldBeFalse();
        ((Result<int, Exception>.Ok)result).Value.ShouldBe(expectedValue);
    }

    [Fact]
    public void RunShouldReturnErrorResultWhenFunctionThrows()
    {
        var       exceptionMessage  = "Test exception";
        Exception expectedException = new InvalidOperationException(exceptionMessage);
        Func<int> failingFunc       = () => throw expectedException;

        var result = Try.Run(failingFunc);

        result.ShouldBeOfType<Result<int, Exception>.Error>();
        Pattern.IsOk(result).ShouldBeFalse();
        Pattern.IsError(result).ShouldBeTrue();
        ((Result<int, Exception>.Error)result).Reason.ShouldBe(expectedException);
        ((Result<int, Exception>.Error)result).Reason.Message.ShouldBe(exceptionMessage);
    }

    [Fact]
    public void RunShouldCaptureSpecificExceptionTypes()
    {
        Func<int> argNullFunc = () => throw new ArgumentNullException("testParam");

        var result = Try.Run(argNullFunc);

        var error = ((Result<int, Exception>.Error)result).Reason;
        error.ShouldBeOfType<ArgumentNullException>();
        (error as ArgumentNullException)?.ParamName.ShouldBe("testParam");
    }

    [Fact]
    public async Task RunAsyncShouldReturnOkResultWhenAsyncFunctionSucceedsAsync()
    {
        var expectedValue = "async result";
        var successFunc   = () => Task.FromResult(expectedValue);

        var result = await Try.RunAsync(successFunc);

        result.ShouldBeOfType<Result<string, Exception>.Ok>();
        Pattern.IsOk(result).ShouldBeTrue();
        Pattern.IsError(result).ShouldBeFalse();
        ((Result<string, Exception>.Ok)result).Value.ShouldBe(expectedValue);
    }

    [Fact]
    public async Task RunAsyncShouldReturnErrorResultWhenAsyncFunctionThrowsAsync()
    {
        var       exceptionMessage  = "Async test exception";
        Exception expectedException = new InvalidOperationException(exceptionMessage);
        var       failingFunc       = () => Task.FromException<string>(expectedException);

        var result = await Try.RunAsync(failingFunc);

        result.ShouldBeOfType<Result<string, Exception>.Error>();
        Pattern.IsOk(result).ShouldBeFalse();
        Pattern.IsError(result).ShouldBeTrue();
        ((Result<string, Exception>.Error)result).Reason.ShouldBe(expectedException);
        ((Result<string, Exception>.Error)result).Reason.Message.ShouldBe(exceptionMessage);
    }

    [Fact]
    public async Task RunAsyncShouldCaptureExceptionFromAsyncAwaitOperationAsync()
    {
        Func<Task<int>> failingAsyncFunc = async () =>
                                           {
                                               await Task.Delay(1);

                                               throw new TimeoutException("Operation timed out");
                                           };

        var result = await Try.RunAsync(failingAsyncFunc);

        Pattern.IsError(result).ShouldBeTrue();
        ((Result<int, Exception>.Error)result).Reason.ShouldBeOfType<TimeoutException>();
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
        error.ShouldBeOfType<CustomTestException>();
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
        error.ShouldBeOfType<CustomTestException>();
        error.ShouldBeSameAs(customException);
    }

    // Custom exception for testing
    private class CustomTestException(string message) : Exception(message);
}
