using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleBus;
using SimpleBus.InMemoryTransport;

public class MessageProcessor : BackgroundService
{
    private readonly ChannelRegistry _channelRegistry;
    private readonly MessageDispatcher _messageDispatcher;
    private readonly ILogger<MessageProcessor> _logger;
    private readonly RegisteredConsumersByMessageType _consumersByMessageType;

    public MessageProcessor(
        ChannelRegistry channelRegistry,
        MessageDispatcher messageDispatcher,
        ILogger<MessageProcessor> logger,
        RegisteredConsumersByMessageType consumersByMessageType)
    {
        _channelRegistry = channelRegistry;
        _messageDispatcher = messageDispatcher;
        _logger = logger;
        _consumersByMessageType = consumersByMessageType;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var messageTypes = _consumersByMessageType.GetMessagesTypes();
        var tasks = messageTypes.Select(type => ProcessMessagesOfTypeAsync(type, stoppingToken));
        await Task.WhenAll(tasks);
    }

    private async Task ProcessMessagesOfTypeAsync(Type messageType, CancellationToken stoppingToken)
    {
        var method = typeof(MessageProcessor)
            .GetMethod(nameof(ProcessChannel), BindingFlags.Instance | BindingFlags.NonPublic)
            ?.MakeGenericMethod(messageType);
            
        await (Task) method.Invoke(this, [stoppingToken]);
    }
    
    private async Task ProcessChannel<T>(CancellationToken stoppingToken) where T : class
    {
        var channel = _channelRegistry.GetChannel<T>();
        var reader = channel.Reader;
        
        try
        {
            while (await reader.WaitToReadAsync(stoppingToken))
            {
                while (reader.TryRead(out var message))
                {
                    try
                    {
                        await _messageDispatcher.DispatchAsync(message, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message of type {MessageType}", typeof(T).Name);
                    }
                }
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error in channel processor for {MessageType}", typeof(T).Name);
        }
    }
}