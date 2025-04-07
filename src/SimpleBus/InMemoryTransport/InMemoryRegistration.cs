using System;
using System.Linq;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleBus.InMemoryTransport
{
    public static class InMemoryRegistration
    {
        public static void UsingInMemoryTransport(this IBusConfigurator busConfigurator, Action<ITransportConfigurator> configure)
        {
            var configurator = new InMemoryConfigurator(busConfigurator.Services);
            configure.Invoke(configurator);
            
            busConfigurator.Services.AddSingleton<ChannelRegistry>(provider =>
            {
                var consumersByMessageType = provider.GetRequiredService<RegisteredConsumersByMessageType>();
                var messageTypes = consumersByMessageType.GetMessagesTypes();
                var registry = new ChannelRegistry();

                foreach (var messageType in messageTypes)
                {
                    var channelCreatorMethod = typeof(Channel)
                        .GetMethods()
                        .First(m => m.Name == nameof(Channel.CreateUnbounded) &&
                                    m.GetParameters().Length == 1 &&
                                    m.GetParameters()[0].ParameterType == typeof(UnboundedChannelOptions) &&
                                    m.IsGenericMethod)
                        .MakeGenericMethod(messageType);

                    var channel = channelCreatorMethod.Invoke(null, [new UnboundedChannelOptions()]);
                    registry.AddChannel(messageType, channel);
                }

                return registry;
            });

            busConfigurator.Services.AddSingleton<IBus, InMemoryBus>();
            busConfigurator.Services.AddHostedService<MessageProcessor>();
        }
    }
}