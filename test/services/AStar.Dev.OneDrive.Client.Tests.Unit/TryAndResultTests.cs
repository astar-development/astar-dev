using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using AStar.Dev.Functional.Extensions;

namespace AStar.Dev.OneDrive.Client.Tests.Unit
{
    public class TryAndResultTests
    {
        [Fact]
        public void Try_Run_Returns_Ok_OnSuccess()
        {
            var r = Try.Run(() => 5 + 2);
            r.Should().BeOfType<Result<int, Exception>.Ok>();
            ((Result<int, Exception>.Ok)r).Value.Should().Be(7);
        }

        [Fact]
        public void Try_Run_Returns_Error_OnException()
        {
            var ex = new InvalidOperationException("boom");
            var r = Try.Run<int>(() => throw ex);
            r.Should().BeOfType<Result<int, Exception>.Error>();
            ((Result<int, Exception>.Error)r).Reason.Should().Be(ex);
        }

        [Fact]
        public async Task Try_RunAsync_Returns_Ok_OnSuccess()
        {
            var r = await Try.RunAsync(async () => { await Task.Delay(1); return "ok"; });
            r.Should().BeOfType<Result<string, Exception>.Ok>();
            ((Result<string, Exception>.Ok)r).Value.Should().Be("ok");
        }

        [Fact]
        public async Task Try_RunAsync_Returns_Error_OnException()
        {
            var ex = new Exception("async boom");
            var r = await Try.RunAsync<string>(async () => { await Task.Delay(1); throw ex; });
            r.Should().BeOfType<Result<string, Exception>.Error>();
            ((Result<string, Exception>.Error)r).Reason.Should().Be(ex);
        }

        [Fact]
        public void Result_Match_Works_For_Ok()
        {
            var r = new Result<int, Exception>.Ok(99);
            var matched = r.Match(v => v + 1, e => -1);
            matched.Should().Be(100);
        }

        [Fact]
        public async Task Result_MatchAsync_Works_For_Ok()
        {
            var r = new Result<int, Exception>.Ok(10);
            var outv = await r.MatchAsync(async v => { await Task.Delay(1); return v * 2; }, e => -1);
            outv.Should().Be(20);
        }
    }
}
