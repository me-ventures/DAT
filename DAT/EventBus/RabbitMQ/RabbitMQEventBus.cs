using System;
using System.Reactive.Subjects;
using DAT.Configuration;
using DAT.Metrics;
using Newtonsoft.Json;
using Optional;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static System.Text.Encoding;

namespace DAT.EventBus.RabbitMQ
{
    public class RabbitMQEventBus : AbstractEventBus
    {
        private readonly DATConfiguration _configuration;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQEventBus(DATConfiguration configuration, IMetricsClient client) 
            : this(configuration, new EventbusConfiguration(){ Hostname = "localhost", Username = "guest", Password = "guest", VirtualHost = "/", Port = 5672 }, client)
        {
            
        }

        public RabbitMQEventBus(DATConfiguration configuration, EventbusConfiguration options, IMetricsClient client): base(client)
        {
            _configuration = configuration;

            int port = 5672;
            
            if (options.Port != null)
            {
                port = options.Port.Value;
            }

            string virtualHost = "/";
            if (options.VirtualHost != null)
            {
                virtualHost = options.VirtualHost;
            }
            
            _connectionFactory = new ConnectionFactory
            {
                UserName = options.Username,
                Password = options.Password,
                VirtualHost = virtualHost,
                HostName = options.Hostname,
                Port = port
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

        protected override IObservable<T> InternalSubscribe<T>(string eventName)
        {
            Tuple<string, string> bus = splitQueueExchange(eventName);
            Subject<T> subject = new Subject<T>();
            
            string queueName = $"{_configuration.Name}.{eventName}";
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, bus.Item1, bus.Item2, null);

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, args) =>
            {
                byte[] body = args.Body;

                T message = DecodeMessage<T>(body);
                subject.OnNext(message);
                
                _channel.BasicAck(args.DeliveryTag, false);
            };
            
            String consumerTag = _channel.BasicConsume(queueName, false, consumer);
            
            return subject;

        }

        protected override void InternalSubscribe<T>(string eventName, Func<T, bool> handler)
        {
            Tuple<string, string> bus = splitQueueExchange(eventName);
            
            string queueName = $"{_configuration.Name}.{eventName}";
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, bus.Item1, bus.Item2, null);

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, args) =>
            {
                byte[] body = args.Body;

                T message = DecodeMessage<T>(body);
                bool result = handler(message);

                if (result)
                {
                    _channel.BasicAck(args.DeliveryTag, false);    
                }
                else
                {
                    _channel.BasicNack(args.DeliveryTag, false, true);
                }
                
            };
            
            String consumerTag = _channel.BasicConsume(queueName, false, consumer);
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

        private T DecodeMessage<T>(byte[] message)
        {
            return JsonConvert.DeserializeObject<T>(UTF8.GetString(message));    
        }
    }
}