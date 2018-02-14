using System;
using DAT.Configuration;
using Newtonsoft.Json;
using Optional;
using RabbitMQ.Client;
using static System.Text.Encoding;

namespace DAT.EventBus.RabbitMQ
{
    public class RabbitMQEventBus : AbstractEventBus
    {
        private readonly DATConfiguration _configuration;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQEventBus(DATConfiguration configuration) : this(configuration, new RabbitMQConnectionOptions{ Hostname = "localhost", Username = "guest", Password = "guest", VirtualHost = "/", Port = 5672 })
        {
            
        }

        public RabbitMQEventBus(DATConfiguration configuration, RabbitMQConnectionOptions options)
        {
            _configuration = configuration;
            _connectionFactory = new ConnectionFactory
            {
                UserName = options.Username,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                HostName = options.Hostname,
                Port = options.Port
            };

            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        
        protected override Option<T> InternalGet<T>(string eventName)
        {
            Tuple<string, string> bus = splitQueueExchange(eventName);
            
            DeclareExchange(bus.Item1);
            
            // TODO: Declare Queue
            string queueName = $"{_configuration.Name}.{eventName}";
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, bus.Item1, bus.Item2, null);
            
            BasicGetResult result = _channel.BasicGet(queueName, true);
            

            if (result == null)
            {
                return Option.None<T>();
            }

            byte[] body = result.Body;

            T decodedMessage = JsonConvert.DeserializeObject<T>(UTF8.GetString(body));

            return Option.Some(decodedMessage);
        }

        protected override T InternalSubscribe<T>(string eventName)
        {
            throw new NotImplementedException();
        }

        protected override void InternalPublish<T>(string eventName, T @event)
        {
            Tuple<string, string> bus = splitQueueExchange(eventName);

            byte[] message = UTF8.GetBytes(JsonConvert.SerializeObject(@event));
            
            DeclareExchange(bus.Item1);
            
            _channel.BasicPublish(bus.Item1, bus.Item2, null, message);
        }

        private Tuple<string, string> splitQueueExchange(string complete)
        {
            int splitNumber = complete.IndexOf('.');
            string exchange = complete.Substring(0, splitNumber);
            string topic = complete.Substring(splitNumber + 1, (complete.Length - 1) - splitNumber);
            
            return new Tuple<string, string>(exchange, topic);
        }

        private void DeclareExchange(string exchange)
        {
            _channel.ExchangeDeclare(exchange, "direct", true);
        }
    }
}