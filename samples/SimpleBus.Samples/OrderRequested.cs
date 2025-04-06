using System;

namespace SimpleBus.Samples;

public record OrderRequested
{
    public Guid OrderId { get; set; }
    public DateTime RequestedAt { get; set; }
}