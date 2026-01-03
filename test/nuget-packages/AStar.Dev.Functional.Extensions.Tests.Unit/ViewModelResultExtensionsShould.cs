namespace AStar.Dev.Functional.Extensions.Tests.Unit;

public class ViewModelResultExtensionsShould
{
    [Fact]
    public void ApplyInvokeOnSuccessForOk()
    {
        // Arrange
        var ok = new Result<string, Exception>.Ok("value");
        string? captured = null;
        var errorCalled = false;

        // Act
        ok.Apply(v => captured = v, _ => errorCalled = true);

        // Assert
        captured.ShouldBe("value");
        errorCalled.ShouldBeFalse();
    }

    [Fact]
    public void ApplyInvokeOnErrorWhenErrorAndHandlerProvided()
    {
        // Arrange
        var ex = new InvalidOperationException("boom");
        var err = new Result<int, Exception>.Error(ex);
        Exception? captured = null;
        var successCalled = false;

        // Act
        err.Apply(_ => successCalled = true, e => captured = e);

        // Assert
        successCalled.ShouldBeFalse();
        captured.ShouldBeSameAs(ex);
    }

    [Fact]
    public void ApplyDoNothingOnErrorWhenNoOnErrorProvided()
    {
        // Arrange
        var err = new Result<int, Exception>.Error(new Exception("x"));
        var successCalled = false;

        // Act
        err.Apply(_ => successCalled = true);

        // Assert
        successCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task ApplyAsyncWithActionHandlersInvokesSuccessForOk()
    {
        // Arrange
        Task<Result<int, Exception>> TaskOk()
        {
            return Task.FromResult<Result<int, Exception>>(new Result<int, Exception>.Ok(7));
        }

        var captured = 0;
        var errorCalled = false;

        // Act
        await TaskOk().ApplyAsync(v => captured = v, _ => errorCalled = true);

        // Assert
        captured.ShouldBe(7);
        errorCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task ApplyAsyncWithActionHandlersInvokesErrorForError()
    {
        // Arrange
        var ex = new Exception("bad");

        Task<Result<int, Exception>> TaskErr()
        {
            return Task.FromResult<Result<int, Exception>>(new Result<int, Exception>.Error(ex));
        }

        Exception? captured = null;
        var successCalled = false;

        // Act
        await TaskErr().ApplyAsync(_ => successCalled = true, e => captured = e);

        // Assert
        successCalled.ShouldBeFalse();
        captured.ShouldBeSameAs(ex);
    }

    [Fact]
    public async Task ApplyAsyncWithAsyncHandlersInvokesSuccessForOk()
    {
        // Arrange
        Task<Result<string, Exception>> TaskOk()
        {
            return Task.FromResult<Result<string, Exception>>(new Result<string, Exception>.Ok("ok"));
        }

        string? captured = null;
        var errorCalled = false;

        // Act
        await TaskOk().ApplyAsync(async v =>
        {
            await Task.Delay(1);
            captured = v;
        }, async _ =>
        {
            await Task.Delay(1);
            errorCalled = true;
        });

        // Assert
        captured.ShouldBe("ok");
        errorCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task ApplyAsyncWithAsyncHandlersInvokesErrorWhenProvided()
    {
        // Arrange
        var ex = new InvalidOperationException("nope");

        Task<Result<bool, Exception>> TaskErr()
        {
            return Task.FromResult<Result<bool, Exception>>(new Result<bool, Exception>.Error(ex));
        }

        Exception? captured = null;
        var successCalled = false;

        // Act
        await TaskErr().ApplyAsync(async _ =>
        {
            await Task.Delay(1);
            successCalled = true;
        }, async e =>
        {
            await Task.Delay(1);
            captured = e;
        });

        // Assert
        successCalled.ShouldBeFalse();
        captured.ShouldBeSameAs(ex);
    }

    [Fact]
    public async Task ApplyAsyncWithAsyncHandlersDoNothingOnErrorWhenNoOnErrorProvided()
    {
        // Arrange
        Task<Result<int, Exception>> TaskErr()
        {
            return Task.FromResult<Result<int, Exception>>(new Result<int, Exception>.Error(new Exception("err")));
        }

        var successCalled = false;

        // Act
        await TaskErr().ApplyAsync(async _ =>
        {
            await Task.Delay(1);
            successCalled = true;
        });

        // Assert
        successCalled.ShouldBeFalse();
    }
}
