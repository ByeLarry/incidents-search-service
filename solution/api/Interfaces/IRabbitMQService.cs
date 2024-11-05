namespace api.Interfaces
{
    public interface IRabbitMQService : IDisposable
    {
        void SendMessage(string message, string queueName, string exchange);

        void ReceiveMessage(string queueName, Action<string> onMessageReceived);

        void ReceiveMessageRpc(string queueName, string exchange, Func<string, string> onMessageReceived);
    }

}
