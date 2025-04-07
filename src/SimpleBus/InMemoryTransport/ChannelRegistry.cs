using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace SimpleBus.InMemoryTransport;

public class ChannelRegistry
{
    private readonly Dictionary<Type, object> _channels = new();

    public void AddChannel(Type messageType, object channel)
    {
        _channels[messageType] = channel;
    }

    public Channel<T> GetChannel<T>() where T : class
    {
        if (_channels.TryGetValue(typeof(T), out var channel))
        {
            return (Channel<T>)channel;
        }
        
        throw new InvalidOperationException($"No channel registered for message type {typeof(T)}");
    }
}