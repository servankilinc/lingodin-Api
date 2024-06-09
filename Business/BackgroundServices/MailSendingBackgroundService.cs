using Business.Abstract;
using Business.MessageBroker;
using Core.Utils.Mail;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Business.BackgroundServices;

public class MailSendingBackgroundService : BackgroundService
{
    private IConnection? _connection; 
    private IModel? _channel;
    private readonly ConnectionFactory _connectionFactory;

    private readonly IMailService _mailService;
    private readonly ILogger<MailSendingBackgroundService> _logger;
    public MailSendingBackgroundService(ConnectionFactory connectionFactory, ILogger<MailSendingBackgroundService> logger, IMailService mailService)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _mailService = mailService;
    }


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: MailMQKeys.exchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false
        );

        _channel.QueueDeclare(
            queue: MailMQKeys.queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueBind(
            queue: MailMQKeys.queueName,
            exchange: MailMQKeys.exchangeName,
            routingKey: MailMQKeys.routingKey,
            null
        );
        _channel.BasicQos(0, 1, false);

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        _channel.BasicConsume(
            queue: MailMQKeys.queueName,
            autoAck: false,
            consumer: consumer
        );

        consumer.Received += ConsumerRecived;

        return Task.CompletedTask;
    }

    private async Task ConsumerRecived(object model, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var mailModel = JsonConvert.DeserializeObject<MailSendModel>(Encoding.UTF8.GetString(eventArgs.Body.ToArray()));
            await _mailService.SendMailAsync(mailSendModel: mailModel!);

            _channel!.BasicAck(eventArgs.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Mail Sending By Background Service throw an error => {ex.Message?? ex.InnerException!.Message?? ""}");
        } 
    }


    public override void Dispose()
    {
        if(_channel != null)
        {
            _channel.Close();
            _channel.Dispose();
        }

        if(_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
        }

        base.Dispose();
    }
}