using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleBus.InMemoryTransport
{
    public class MessageProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageDispatcher _messageDispatcher;
        private readonly ILogger<MessageProcessor> _logger;
        private readonly List<Task> _workers = [];
        
        public MessageProcessor(IServiceProvider serviceProvider, MessageDispatcher messageDispatcher, ILogger<MessageProcessor> logger)
        {
            _serviceProvider = serviceProvider;
            _messageDispatcher = messageDispatcher;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var registeredConsumersByMessageTypes = 
                _serviceProvider.GetRequiredService<RegisteredConsumersByMessageType>();
            var messagesTypes = registeredConsumersByMessageTypes.GetMessagesTypes();
            
            var channels = new List<object>();

            foreach (var messageType in messagesTypes)
            {
                var channel = _serviceProvider.GetService(typeof(Channel<>).MakeGenericType(messageType));
                channels.Add(channel);
            }
            
            foreach (var channel in channels)
            {
                var readerProperty = channel.GetType().GetProperty("Reader");
                var reader = readerProperty.GetValue(channel);

                var waitToReadAsyncMethod = reader.GetType().GetMethod("WaitToReadAsync", [typeof(CancellationToken)]);
                var tryReadMethod = reader.GetType().GetMethod("TryRead", new[] { reader.GetType().GetGenericArguments()[0].MakeByRefType() });

                var task = Task.Run(async () =>
                {
                    while ((bool) await (dynamic) waitToReadAsyncMethod.Invoke(reader, [stoppingToken]))
                    {
                        var message = (object) Activator.CreateInstance(reader.GetType().GetGenericArguments()[0]);
                        var parameters = new[] { message };
                        while ((bool)tryReadMethod.Invoke(reader, parameters))
                        {
                            await _messageDispatcher.DispatchAsync(parameters[0], stoppingToken);
                        }
                    }
                }, stoppingToken);

                _workers.Add(task);
            }
            
            await Task.WhenAll(_workers);
        }
    }
}