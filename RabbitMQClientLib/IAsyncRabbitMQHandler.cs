using System.Threading.Tasks;

namespace RabbitMQClientLib
{
    public interface IAsyncRabbitMQHandler
    {
        Task OnReceive(string msg);
    }
}