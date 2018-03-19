using System;
using System.Reactive.Linq;
using DAT.Metrics;
using Optional;

namespace DAT.EventBus
{
    public abstract class AbstractEventBus : IEventBus
    {
        private readonly IMetricsClient _clientMetrics;

        protected AbstractEventBus(IMetricsClient clientMetrics)
        {
            _clientMetrics = clientMetrics;
        }
        
        public void Publish<T>(string eventName, T @event)
        {
            _clientMetrics.Counter($"messages.publish", 1);
            
            InternalPublish(eventName, @event);
        }

        public Option<T> Get<T>(string eventName)
        {
            Option<T> internalGet = InternalGet<T>(eventName);

            if (internalGet.HasValue)
            {
                _clientMetrics.Counter($"messages.{eventName}", 1);
            }

            return internalGet;
        }

        public IObservable<T> Subscribe<T>(string eventName)
        {
            return InternalSubscribe<T>(eventName)
                        .Do(obj => _clientMetrics.Counter($"messages.{eventName}", 1));
        }

        public void Subscribe<T>(string eventName, Func<T, bool> handler)
        {
            Func<T, bool> wrapper = arg =>
            {
                _clientMetrics.Counter($"messages.{eventName}", 1);
                
                return handler(arg);
            };
            
            InternalSubscribe<T>(eventName, wrapper);
        }

        protected abstract Option<T> InternalGet<T>(string eventName);

        protected abstract IObservable<T> InternalSubscribe<T>(string eventName);

        protected abstract void InternalSubscribe<T>(string eventName, Func<T, bool> handler);

        protected abstract void InternalPublish<T>(string eventName,T @event);
    }
}