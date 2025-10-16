using System.Text;
using AStar.Dev.Utilities;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AStar.Dev.Api.Usage.Sdk;

/// <summary>
/// </summary>
public sealed class Send(IConnection connection, IOptions<ApiUsageConfiguration> usageConfiguration)
{
    /// <summary>
    /// </summary>
    /// <param name="usageEvent"></param>
    /// <param name="cancellationToken"></param>
    public async Task SendUsageEventAsync(ApiUsageEvent usageEvent, CancellationToken cancellationToken)
    {
        var config = usageConfiguration.Value;

        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(config.QueueName, true, false, false, cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(usageEvent.ToJson());

        await channel.BasicPublishAsync(string.Empty, config.QueueName, body, cancellationToken);
    }
}
