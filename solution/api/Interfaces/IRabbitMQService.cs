namespace api.Interfaces
{
    public interface IRabbitMQService : IDisposable
    {
        public void SendMessage(string message, string queueName, string exchange = "");

        public void ReceiveMessage(string queueName, Action<string> onMessageReceived);

    }
}
