using System;
using System.Threading.Tasks;
using Microsoft.Graph;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.OneDrive.Client.Services;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.Fakes
{
    internal sealed class FakeLoginService : ILoginService
    {
        private readonly GraphServiceClient _client;

        public FakeLoginService(GraphServiceClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public bool IsSignedIn => true;

        public Task<Result<GraphServiceClient, Exception>> SignInAsync()
        {
            return Task.FromResult<Result<GraphServiceClient, Exception>>(new Result<GraphServiceClient, Exception>.Ok(_client));
        }

        public Task<Result<bool, Exception>> SignOutAsync()
        {
            return Task.FromResult<Result<bool, Exception>>(new Result<bool, Exception>.Ok(true));
        }
    }
}
