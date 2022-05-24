using System.Threading.Tasks;
using DotNetCore.CAP;

namespace mongodb_rabbitmq.Consumers
{
    public interface IConsumer<T> where T : class
    {
        Task ConsumeMessage(T message, [FromCap] CapHeader header);
    }
}