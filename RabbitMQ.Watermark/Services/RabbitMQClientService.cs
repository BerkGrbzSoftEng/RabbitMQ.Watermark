using RabbitMQ.Client;

namespace RabbitMQ.Watermark.Services
{
    public class RabbitMQClientService:IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger<RabbitMQClientService> _logger;

        public static string ExChangeName = "ImageDirectExchange";
        public static string RoutingWaterMark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";
       
        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory; 
            _logger = logger; 
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if (_channel is {IsOpen:true})
            {
                return _channel;
            }

            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExChangeName,type:ExchangeType.Direct,true,false);
            _channel.QueueDeclare(QueueName, true, false, false,null);
            _channel.QueueBind(ExChangeName,QueueName,RoutingWaterMark);
            _logger.LogInformation("RabbitMQ ile baglantı kuruldu... ");
            return _channel;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose(); 
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ ile baglantı koptu... ");
        }
    }
}
