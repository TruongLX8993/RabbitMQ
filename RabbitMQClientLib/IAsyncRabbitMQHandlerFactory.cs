namespace RabbitMQClientLib
{
    public interface IAsyncRabbitMQHandlerFactory
    {
        IAsyncRabbitMQHandler Create();
    }
}