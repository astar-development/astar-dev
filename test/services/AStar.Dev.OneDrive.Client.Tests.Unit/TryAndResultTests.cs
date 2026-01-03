using System;
using System.Threading.Tasks;

using Xunit;
using AStar.Dev.Functional.Extensions;

namespace AStar.Dev.OneDrive.Client.Tests.Unit
{
    public sealed class TryAndResultTests
    {
        [Fact]
        public void Try_Run_Returns_Ok_OnSuccess()
        {
            Result<int, Exception> r = Try.Run(() => 5 + 2);
            r.ShouldBeOfType<Result<int, Exception>.Ok>();
            ((Result<int, Exception>.Ok)r).Value.ShouldBe(7);
        }

        [Fact]
        public void Try_Run_Returns_Error_OnException()
        {
            var ex = new InvalidOperationException("boom");
            Result<int, Exception> r = Try.Run<int>(() => throw ex);
            r.ShouldBeOfType<Result<int, Exception>.Error>();
            ((Result<int, Exception>.Error)r).Reason.ShouldBe(ex);
        }

        [Fact]
        public async Task Try_RunAsync_Returns_Ok_OnSuccess()
        {
            Result<string, Exception> r = await Try.RunAsync(async () => { await Task.Delay(1); return "ok"; });
            r.ShouldBeOfType<Result<string, Exception>.Ok>();
            ((Result<string, Exception>.Ok)r).Value.ShouldBe("ok");
        }

        [Fact]
        public async Task Try_RunAsync_Returns_Error_OnException()
        {
            var ex = new Exception("async boom");
            Result<string, Exception> r = await Try.RunAsync<string>(async () => { await Task.Delay(1); throw ex; });
            r.ShouldBeOfType<Result<string, Exception>.Error>();
            ((Result<string, Exception>.Error)r).Reason.ShouldBe(ex);
        }

        [Fact]
        public void Result_Match_Works_For_Ok()
        {
            var r = new Result<int, Exception>.Ok(99);
            var matched = r.Match(v => v + 1, e => -1);
            matched.ShouldBe(100);
        }

        [Fact]
        public async Task Result_MatchAsync_Works_For_Ok()
        {
            var r = new Result<int, Exception>.Ok(10);
            var outv = await r.MatchAsync(async v => { await Task.Delay(1); return v * 2; }, e => -1);
            outv.ShouldBe(20);
        }
    }
}
