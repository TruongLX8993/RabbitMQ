using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQClientLib
{
    public class RabbitMQClient
    {
        private readonly string _uri;
        private IConnection _connection;
        private IModel _channel;


        public RabbitMQClient(string uri)
        {
            _uri = uri;
        }

        public void Connect()
        {
            var connectionFac = new ConnectionFactory {Uri = new Uri(_uri), DispatchConsumersAsync = true};
            _connection = connectionFac.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void DeclareQueue(string queueName)
        {
            _channel.QueueDeclare(queueName, false, false, false, null);
        }

        public void DeclareExchangeAndQueue(string exchangeName,
            string baseQueueName,
            int numberQueue)
        {
            // Declare queues
            var queueNames = new string[numberQueue];
            for (var i = 0; i < numberQueue; i++)
            {
                queueNames[i] = $"{baseQueueName}-{i + 1}";
            }

            foreach (var queueName in queueNames)
            {
                _channel.QueueDeclare(queueName, false, false, false, null);
            }

            // Declare exchange and build queue to exchange
            const string type = "x-consistent-hash";
            var args = new Dictionary<string, object> {{"hash-header", "hash-on"}};
            _channel.ExchangeDeclare(exchangeName, type, false, false, args);
            for (var i = 0; i < numberQueue; i++)
            {
                _channel.QueueBind(queueNames[i], exchangeName, (i + 1).ToString());
            }
        }

        public RabbitMQExchangeSender CreateRabbitMqExchangeSender(string exchangeName)
        {
            return new RabbitMQExchangeSender(_channel, exchangeName);
        }

        public void Close()
        {
            _connection.Close();
        }

        public RabbitMqSimpleSender CreateSender(string exchangeName, string queueName)
        {
            return new RabbitMqSimpleSender(_channel, exchangeName, queueName);
        }

        public void BindHandler(string baseQueueName, int numberQueue, IAsyncRabbitMQHandlerFactory handlerFactory)
        {
            for (var i = 0; i < numberQueue; i++)
            {
                var queueName = $"{baseQueueName}-{i}";
                BindHandler(queueName,handlerFactory.Create());
            }
        }

        public void BindHandler(string queueName, params IAsyncRabbitMQHandler[] handlers)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicQos(0, 10, false);
            foreach (var handler in handlers)
            {
                consumer.Received += async (ch, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var msg = Encoding.UTF8.GetString(body);
                    await handler.OnReceive(msg);
                };
            }

            _channel.BasicConsume(queueName, true, consumer);
        }
    }
}