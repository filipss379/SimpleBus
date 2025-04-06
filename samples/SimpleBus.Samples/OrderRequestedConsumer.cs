using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBus.Samples;

public class OrderRequestedConsumer : IConsumer<OrderRequested>
{
    private readonly MessagesCounter _messagesCounter;

    public OrderRequestedConsumer(MessagesCounter messagesCounter)
    {
        _messagesCounter = messagesCounter;
    }

    public Task Handle(OrderRequested message, CancellationToken cancellationToken)
    {
        _messagesCounter.Increment();
        Console.WriteLine($"{_messagesCounter.GetCount()}. Order requested is being processed for {message.OrderId}.");
        return Task.CompletedTask;
    }
}

public class OrderRequestedConsumer2 : IConsumer<OrderRequested>
{
    private readonly MessagesCounter _messagesCounter;

    public OrderRequestedConsumer2(MessagesCounter messagesCounter)
    {
        _messagesCounter = messagesCounter;
    }

    public Task Handle(OrderRequested message, CancellationToken cancellationToken)
    {
        _messagesCounter.Increment();
        Console.WriteLine($"{_messagesCounter.GetCount()}. Order requested is being processed for {message.OrderId}.");
        return Task.CompletedTask;
    }
}

