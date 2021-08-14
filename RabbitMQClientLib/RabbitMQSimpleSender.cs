using System.Text;
using RabbitMQ.Client;

namespace RabbitMQClientLib
{
    public class RabbitMqSimpleSender
    {
        private readonly IModel _channel;
        private readonly string _exchange;
        private readonly string _queue;
        
        public RabbitMqSimpleSender(IModel channel,string exchange,string queue)
        {
            _channel = channel;
            _exchange = exchange;
            _queue = queue;
        }
        
        

        public void Send(string msg)
        {
            _channel.BasicPublish(_exchange,_queue,null,Encoding.UTF8.GetBytes(msg));
        }
    }
}