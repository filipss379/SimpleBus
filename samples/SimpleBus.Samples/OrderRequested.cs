using System;

namespace SimpleBus.Samples;

public record OrderRequested : IMessage
{
    public Guid OrderId { get; set; }
    public DateTime RequestedAt { get; set; }
}