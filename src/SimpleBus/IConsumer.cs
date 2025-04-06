using System.Threading;
using System.Threading.Tasks;

namespace SimpleBus
{
    public interface IConsumer
    {
        
    }

    public interface IConsumer<in T> : IConsumer where T : IMessage
    {
        Task Handle(T message, CancellationToken cancellationToken);
    }
}