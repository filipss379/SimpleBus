using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SimpleBus.Samples;

public class PublishBackgroundService : BackgroundService
{
    private readonly IBus _bus;
    private readonly MessagesCounter _messagesCounter;
    
    public PublishBackgroundService(IBus bus, MessagesCounter messagesCounter)
    {
        _bus = bus;
        _messagesCounter = messagesCounter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int messagesCount = 10_000;
        List<Task> messages = [];
        for (var i = 0; i < messagesCount; i++)
        {
            var orderRequested = new OrderRequested
            {
                OrderId = Guid.NewGuid(),
                RequestedAt = DateTime.UtcNow
            };

            messages.Add(_bus.Publish(orderRequested));
        }

        for(var i = 0; i < messagesCount; i++)
        {
                   
            var orderProcessed = new OrderProcessed
            {
                OrderId = Guid.NewGuid(),
                ProcessedAt = DateTime.UtcNow
            };
            messages.Add(_bus.Publish(orderProcessed));
        }

        await Task.WhenAll(messages); 
        
        await Task.Delay(3000, stoppingToken);
        Console.WriteLine($"Processed {_messagesCounter.GetCount()} messages.");

    }
}