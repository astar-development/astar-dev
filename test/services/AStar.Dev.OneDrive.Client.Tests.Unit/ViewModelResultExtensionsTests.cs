using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using AStar.Dev.Functional.Extensions;

namespace AStar.Dev.OneDrive.Client.Tests.Unit
{
    public sealed class ViewModelResultExtensionsTests
    {
        [Fact]
        public void Apply_Executes_OnSuccess()
        {
            var result = new Result<int, Exception>.Ok(5);
            int captured = 0;
            Exception? capturedEx = null;

            result.Apply(v => captured = v, ex => capturedEx = ex);

            captured.Should().Be(5);
            capturedEx.Should().BeNull();
        }

        [Fact]
        public async Task ApplyAsync_Executes_OnSuccess()
        {
            Task<Result<int, Exception>> task = Task.FromResult<Result<int, Exception>>(new Result<int, Exception>.Ok(7));
            int captured = 0;

            await task.ApplyAsync(v => captured = v, ex => { throw ex; });

            captured.Should().Be(7);
        }

        [Fact]
        public void ToErrorMessage_Returns_Empty_OnOk()
        {
            var ok = new Result<string, Exception>.Ok("value");
            ok.ToErrorMessage().Should().BeEmpty();
        }

        [Fact]
        public void ToErrorMessage_Returns_Message_OnError()
        {
            var ex = new InvalidOperationException("boom");
            var err = new Result<string, Exception>.Error(ex);
            err.ToErrorMessage().Should().Be("boom");
        }

        [Fact]
        public async Task GetOrThrowAsync_Returns_Value_OnOk()
        {
            Task<Result<int, Exception>> task = Task.FromResult<Result<int, Exception>>(new Result<int, Exception>.Ok(42));
            int v = await task.GetOrThrowAsync();
            v.Should().Be(42);
        }

        [Fact]
        public async Task GetOrThrowAsync_Throws_OnError()
        {
            Task<Result<int, Exception>> task = Task.FromResult<Result<int, Exception>>(new Result<int, Exception>.Error(new InvalidOperationException("fail")));
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await task.GetOrThrowAsync());
        }
    }
}
