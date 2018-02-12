using System;
using Newtonsoft.Json;
using Optional;
using RabbitMQ.Client;
using static System.Text.Encoding;

namespace DAT.EventBus.RabbitMQ
{
    public class RabbitMQEventBus : AbstractEventBus
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQEventBus() : this(new RabbitMQConnectionOptions{ Hostname = "localhost", Username = "guest", Password = "guest", VirtualHost = "/", Port = 5672 })
        {
            
        }

        public RabbitMQEventBus(RabbitMQConnectionOptions options)
        {
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
            
            BasicGetResult result = _channel.BasicGet(bus.Item2, true);
            

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
            string topic = complete.Substring(splitNumber, (complete.Length - 1) - splitNumber);
            
            return new Tuple<string, string>(exchange, topic);
        }

        private void DeclareExchange(string exchange)
        {
            _channel.ExchangeDeclare(exchange, "direct", true);
        }
    }
}