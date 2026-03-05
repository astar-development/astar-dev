namespace AStar.Dev.Functional.Extensions.Tests.Unit;

public class TryExtensionsShould
{
    [Fact]
    public void MapExceptionToErrorResponseUsingBaseExceptionMessage()
    {
        // Arrange
        var inner = new InvalidOperationException("root cause");
        var ex = new Exception("wrapper", inner);
        Result<int, Exception> err = new Result<int, Exception>.Error(ex);

        // Act
        Result<int, ErrorResponse> mapped = err.ToErrorResponse();

        // Assert
        mapped.ShouldBeOfType<Result<int, ErrorResponse>.Error>()
            .Reason.Message.ShouldBe("root cause");
    }

    [Fact]
    public void PassThroughSuccessUnchanged()
    {
        // Arrange
        Result<string, Exception> ok = new Result<string, Exception>.Ok("value");

        // Act
        Result<string, ErrorResponse> mapped = ok.ToErrorResponse();

        // Assert
        mapped.ShouldBeOfType<Result<string, ErrorResponse>.Ok>()
            .Value.ShouldBe("value");
    }

    [Fact]
    public async Task MapExceptionToErrorResponseAsyncUsingBaseExceptionMessage()
    {
        // Arrange
        var inner = new ApplicationException("base message");
        var ex = new Exception("outer", inner);
        var task = Task.FromResult<Result<bool, Exception>>(new Result<bool, Exception>.Error(ex));

        // Act
        Result<bool, ErrorResponse> mapped = await task.ToErrorResponseAsync();

        // Assert
        mapped.ShouldBeOfType<Result<bool, ErrorResponse>.Error>()
            .Reason.Message.ShouldBe("base message");
    }

    [Fact]
    public async Task PassThroughSuccessUnchangedAsync()
    {
        // Arrange
        var task = Task.FromResult<Result<int, Exception>>(new Result<int, Exception>.Ok(42));

        // Act
        Result<int, ErrorResponse> mapped = await task.ToErrorResponseAsync();

        // Assert
        mapped.ShouldBeOfType<Result<int, ErrorResponse>.Ok>()
            .Value.ShouldBe(42);
    }
}
