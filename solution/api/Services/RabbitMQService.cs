using api.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace api.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(IOptions<RabbitMQOptions> options)
        {
            var factory = new ConnectionFactory()
            {
                HostName = options.Value.HostName,
                UserName = options.Value.UserName,
                Password = options.Value.Password
            };
            Console.WriteLine(options.Value.HostName);
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void SendMessage(string message, string queueName, string exchange = "")
        {
            _channel.QueueDeclare(queue: queueName);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange, routingKey: queueName, basicProperties: null, body: body);
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

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }

    public class RabbitMQOptions
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
