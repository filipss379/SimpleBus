using System.Threading.Tasks;

namespace SimpleBus
{
    public interface IBus
    {
        Task Publish<T>(T message) where T : class;
    }
}