using System;
using System.Threading.Tasks;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.OneDrive.Client.Services;
using Microsoft.Graph;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.Fakes
{
    internal class FailingLoginService : ILoginService
    {
        private readonly Exception _reason;

        public FailingLoginService(Exception reason)
        {
            _reason = reason ?? new Exception("login failed");
        }

        public bool IsSignedIn => false;

        public Task<Result<GraphServiceClient, Exception>> SignInAsync()
        {
            return Task.FromResult<Result<GraphServiceClient, Exception>>(new Result<GraphServiceClient, Exception>.Error(_reason));
        }

        public Task<Result<bool, Exception>> SignOutAsync()
        {
            return Task.FromResult<Result<bool, Exception>>(new Result<bool, Exception>.Error(_reason));
        }
    }
}
