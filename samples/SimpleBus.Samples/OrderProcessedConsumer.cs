using System;
using System.Threading.Tasks;

namespace SimpleBus.Samples;

public class OrderProcessedConsumer : IConsumer<OrderProcessed>
{
    private readonly MessagesCounter _messagesCounter;

    public OrderProcessedConsumer(MessagesCounter messagesCounter)
    {
        _messagesCounter = messagesCounter;
    }

    public Task Handle(OrderProcessed message)
    {
        _messagesCounter.Increment();
        Console.WriteLine($"{_messagesCounter.GetCount()}. Order processed is being processed for {message.OrderId}.");
        return Task.CompletedTask;
    }
}

public class OrderProcessed : IMessage
{
    public Guid OrderId { get; set; }
    public DateTimeOffset ProcessedAt { get; set; }
}