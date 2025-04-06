using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleBus
{
    public class MessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(IMessage message, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var messageType = message.GetType();

            var consumerType = typeof(IConsumer<>).MakeGenericType(messageType);
            var consumers = scope.ServiceProvider.GetServices(consumerType);

            foreach (var consumer in consumers)
            {
                var consumeMethod = consumerType.GetMethod("Handle");
                if (consumeMethod != null)
                {
                    await (Task) consumeMethod.Invoke(consumer, [message, cancellationToken]);
                }
            }
        }
    }
}