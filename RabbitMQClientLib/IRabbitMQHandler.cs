using System.Threading.Tasks;

namespace RabbitMQClientLib
{
    public interface IRabbitMQHandler
    {
        void OnReceive(string msg);
    }
}