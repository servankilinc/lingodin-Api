using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Business.MessageBroker;

public class MailMessageBrokerProducer
{
    private readonly ConnectionFactory _connectionFactory;
    public MailMessageBrokerProducer(ConnectionFactory connectionFactory) => _connectionFactory = connectionFactory;

    public void SendByDirectExc<T>(T message)
    {
        using IConnection connection = _connectionFactory.CreateConnection();
        using IModel channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: MailMQKeys.exchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false
        );

        channel.QueueDeclare(
            queue: MailMQKeys.queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        channel.QueueBind(
            queue: MailMQKeys.queueName,
            exchange: MailMQKeys.exchangeName,
            routingKey: MailMQKeys.routingKey,
            null
        );
        
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        channel.BasicPublish(
            exchange: MailMQKeys.exchangeName,
            routingKey: MailMQKeys.routingKey,
            basicProperties: properties,
            body: body
        );
    }
}