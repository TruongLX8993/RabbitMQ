namespace RabbitMQClientLib
{
    public interface IRabbitMQHandlerFactory
    {
        IRabbitMQHandler Create();
    }
}