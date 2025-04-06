using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SimpleBus.InMemoryTransport
{
    public class InMemoryBus : IBus
    {
        private readonly IServiceProvider _serviceProvider; 
        private readonly ILogger<InMemoryBus> _logger;

        public InMemoryBus(IServiceProvider serviceProvider, ILogger<InMemoryBus> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Publish<T>(T message) where T : class
        {
            _logger.LogTrace("Publishing message of type {MessageType}", typeof(T).Name);
            using var scope = _serviceProvider.CreateScope();
            var channel = scope.ServiceProvider.GetRequiredService<Channel<T>>();
            await channel.Writer.WriteAsync(message);
        }
    }
}