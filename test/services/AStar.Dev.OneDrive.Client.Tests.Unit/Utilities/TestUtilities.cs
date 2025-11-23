using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.Utilities
{
    internal static class TestUtilities
    {
        public static GraphServiceClient CreateGraphClient(Func<RequestInformation, Type, CancellationToken, Task<object?>> responder)
        {
            var adapter = new Fakes.FakeRequestAdapter(responder);
            return new GraphServiceClient(adapter);
        }

        public static MemoryStream StreamFromString(string content)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(content ?? string.Empty));
        }
    }
}
