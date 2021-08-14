using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQClientLib;

namespace RabbitMQConsoleApp
{
    public class TestHandler : IAsyncRabbitMQHandler
    {
        private readonly string name;

        public TestHandler(string name)
        {
            this.name = name;
        }

        public Task OnReceive(string msg)
        {
            Console.WriteLine(name + ":" + msg);
            return Task.Delay(500);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var rabbiMQConsumerClient = new RabbitMQClient("amqp://admin:admin@192.168.1.94:5672");
            rabbiMQConsumerClient.Connect();
            rabbiMQConsumerClient.DeclareQueue("test-1");
            rabbiMQConsumerClient.DeclareQueue("test-2");
            rabbiMQConsumerClient.DeclareQueue("test-3");
            rabbiMQConsumerClient.DeclareQueue("test-4");
            rabbiMQConsumerClient.DeclareQueue("test-6");
            
            var rabbitMqClient = new RabbitMQClient("amqp://admin:admin@192.168.1.94:5672");
            rabbitMqClient.Connect();
            rabbitMqClient.DeclareExchangeAndQueue("test","test",5);
            var sender = rabbitMqClient.CreateRabbitMqExchangeSender("test");
            for (var i = 0; i < 500; i++)
            {
                sender.Send(i.ToString(),i.ToString());
            }
            
        }

        static void SimpleTest()
        {
            var rabbitMqClient = new RabbitMQClient("amqp://admin:admin@192.168.1.94:5672");
            rabbitMqClient.Connect();
            rabbitMqClient.DeclareQueue("Test");
            var sender = rabbitMqClient.CreateSender("", "Test");
            for (var i = 0; i < 500; i++)
            {
                sender.Send(i.ToString());
            }

            rabbitMqClient.BindHandler("Test",
                new TestHandler("h1"),
                new TestHandler("h2"));
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}