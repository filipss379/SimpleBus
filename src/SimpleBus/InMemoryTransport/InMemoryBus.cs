using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SimpleBus.InMemoryTransport
{
    public class InMemoryBus : IBus
    {
        private readonly ChannelRegistry _channelRegistry; 
        private readonly ILogger<InMemoryBus> _logger;

        public InMemoryBus(ChannelRegistry channelRegistry, ILogger<InMemoryBus> logger)
        {
            _logger = logger;
            _channelRegistry = channelRegistry;
        }

        public async Task Publish<T>(T message) where T : class
        {
            _logger.LogTrace("Publishing message of type {MessageType}", typeof(T).Name);
            var channel = _channelRegistry.GetChannel<T>();
            await channel.Writer.WriteAsync(message);
        }
    }
}