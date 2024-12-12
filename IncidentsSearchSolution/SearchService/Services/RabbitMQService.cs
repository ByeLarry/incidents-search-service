using SearchService.Interfaces;
using SearchService.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SearchService.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public const string defaultQueue = "search_queue";
        public const string defaultExchange = "";

        public RabbitMQService(IOptions<RabbitMQOptions> options)
        {
            var factory = new ConnectionFactory()
            {
                HostName = options.Value.HostName,
                UserName = options.Value.UserName,
                Password = options.Value.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: defaultQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void SendMessage(string message, string queueName, string routingKey = "", string exchange = "")
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange, routingKey, basicProperties: null, body: body);
        }

        public void ReceiveMessage(string queueName, Action<string> onMessageReceived)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                onMessageReceived(message);
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void ReceiveMessageRpc(string queueName, string exchange, Func<string, Task<string>> onMessageReceived)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var response = await onMessageReceived(message);

                var props = _channel.CreateBasicProperties();
                props.CorrelationId = ea.BasicProperties.CorrelationId;
                var responseBytes = Encoding.UTF8.GetBytes(response);

                _channel.BasicPublish(
                    exchange: exchange,
                    routingKey: ea.BasicProperties.ReplyTo,
                    basicProperties: props,
                    body: responseBytes);
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
