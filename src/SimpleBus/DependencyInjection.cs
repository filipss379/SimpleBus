using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SimpleBus.Utils;

namespace SimpleBus
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSimpleBus(this IServiceCollection services, Action<IBusConfigurator> configure)
        {
            services.AddSingleton<MessageDispatcher>();
            var configurator = new BusConfigurator(services);
            configure.Invoke(configurator);
            return services;
        }  
    }

    public interface IBusConfigurator
    {
        IServiceCollection Services { get; }
        void AddConsumers(Assembly assembly);
    }

    public class BusConfigurator : IBusConfigurator
    {
        public IServiceCollection Services { get; }

        public BusConfigurator(IServiceCollection services)
        {
            Services = services;
        }

        public void AddConsumers(Assembly assembly)
        {
            var consumersTypes = assembly.FindConsumers();
            
            Services.AddSingleton<RegisteredConsumersByMessageType>(_ => new RegisteredConsumersByMessageType(consumersTypes));

            foreach (var consumerType in consumersTypes)
            {
                var handledMessageTypes = consumerType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
                    .Select(i => i.GetGenericArguments()[0]);

                foreach (var messageType in handledMessageTypes)
                {
                    var serviceType = typeof(IConsumer<>).MakeGenericType(messageType);
                    Services.AddTransient(serviceType, consumerType);
                }
            }
        }
    }

    public interface ITransportConfigurator
    {
        void ReceiveEndpoint(string queueName, Action<ConfigurationContext> context);
    }

    public class ConfigurationContext
    {
        private readonly IServiceCollection _services;
        private readonly string _queueName;

        public ConfigurationContext(IServiceCollection services, string queueName)
        {
            _services = services;
            _queueName = queueName;
        }

        public void RegisterConsumer<T>() where T : IConsumer
        {
            // _services.AddSingleton<T>();
        }
        
    }
} 