using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBus.Samples;

public class OrderProcessedConsumer : IConsumer<OrderProcessed>
{
    private readonly MessagesCounter _messagesCounter;

    public OrderProcessedConsumer(MessagesCounter messagesCounter)
    {
        _messagesCounter = messagesCounter;
    }

    public Task Handle(OrderProcessed message, CancellationToken cancellationToken)
    {
        _messagesCounter.Increment();
        Console.WriteLine($"{_messagesCounter.GetCount()}. Order processed is being processed for {message.OrderId}.");
        return Task.CompletedTask;
    }
}

public class OrderProcessed
{
    public Guid OrderId { get; set; }
    public DateTimeOffset ProcessedAt { get; set; }
}