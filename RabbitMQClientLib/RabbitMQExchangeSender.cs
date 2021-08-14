using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQClientLib
{
    public class RabbitMQExchangeSender
    {
        private readonly IModel _channel;
        private readonly string _exchangeName;

        public RabbitMQExchangeSender(IModel channel, string exchangeName)
        {
            _channel = channel;
            _exchangeName = exchangeName;
        }

        public void Send(string key, string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            var pro = _channel.CreateBasicProperties();
            pro.Headers = new Dictionary<string, object>()
            {
                {"hash-on", key}
            };
            _channel.BasicPublish(_exchangeName, string.Empty, pro, bytes);

        }
    }
}