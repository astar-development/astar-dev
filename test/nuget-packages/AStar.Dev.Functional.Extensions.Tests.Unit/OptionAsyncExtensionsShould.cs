namespace AStar.Dev.Functional.Extensions.Tests.Unit;

public class OptionAsyncExtensionsShould
{
    private static Task<Option<string>> GetUserAsync(bool found) => Task.FromResult(found ? new Option<string>.Some("Alice") :  Option.None<string>());

    [Fact]
    public async Task MapAsync_ShouldTransformValue_WhenSome()
    {
        var result = await GetUserAsync(true).MapAsync(name => name.ToUpper());
        var some   = result.ShouldBeOfType<Option<string>.Some>();
        some.Value.ShouldBe("ALICE");
    }

    [Fact]
    public async Task MapAsync_ShouldPreserveNone()
    {
        var result = await GetUserAsync(false).MapAsync(name => name.ToUpper());
        result.ShouldBeOfType<Option<string>.None>();
    }

    [Fact]
    public async Task BindAsync_ShouldChainTasks_WhenSome()
    {
        Task<Option<string>> ValidateAsync(string name)
        {
            return Task.FromResult<Option<string>>(name == "Alice"
                                                       ? new Option<string>.Some("Valid")
                                                       :  Option.None<string>());
        }

        var result = await GetUserAsync(true).BindAsync(ValidateAsync);
        var some   = result.ShouldBeOfType<Option<string>.Some>();
        some.Value.ShouldBe("Valid");
    }

    [Fact]
    public async Task MatchAsync_ShouldHandleSome()
    {
        var result = await GetUserAsync(true).MatchAsync(
                                                         name => Task.FromResult($"Hello {name}"),
                                                         () => Task.FromResult("User not found")
                                                        );

        result.ShouldBe("Hello Alice");
    }

    [Fact]
    public async Task MatchAsync_ShouldHandleNone()
    {
        var result = await GetUserAsync(false).MatchAsync(
                                                          name => Task.FromResult($"Hello {name}"),
                                                          () => Task.FromResult("User not found")
                                                         );

        result.ShouldBe("User not found");
    }
}
