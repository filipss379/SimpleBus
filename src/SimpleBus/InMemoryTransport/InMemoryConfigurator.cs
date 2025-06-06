using System;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleBus.InMemoryTransport
{
    public class InMemoryConfigurator : ITransportConfigurator
    {
        public readonly IServiceCollection _services;

        public InMemoryConfigurator(IServiceCollection services)
        {
            _services = services;
        }

        public void ConfigureQueue(string queueName, Action<ConfigurationContext> context)
        {
            Console.WriteLine($"Registering receive endpoint for queue {queueName}");

            var configContext = new ConfigurationContext(_services, queueName);
            context.Invoke(configContext);
        }

        public void MapToQueue<T>(string orderRequested)
        {
            throw new NotImplementedException();
        }
    }
}