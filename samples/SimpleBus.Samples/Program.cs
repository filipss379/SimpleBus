using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleBus;
using SimpleBus.InMemoryTransport;
using SimpleBus.Samples;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSimpleBus(cfg =>
{
    cfg.AddConsumers(typeof(Program).Assembly);
    
    cfg.UsingInMemoryTransport(configurator =>
    {
        configurator.ReceiveEndpoint("order-requested", ctx =>
        {
            ctx.RegisterConsumer<OrderRequestedConsumer>();
        });
    });
});

builder.Services.AddSingleton<MessagesCounter>();

builder.Services.AddHostedService<PublishBackgroundService>();

var host = builder.Build();
await host.RunAsync();

public class MessagesCounter
{
    private int _count;

    public void Increment()
    {
        Interlocked.Increment(ref _count);
    }

    public int GetCount()
    {
        return _count;
    }
}