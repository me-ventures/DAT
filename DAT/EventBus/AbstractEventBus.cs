using System;
using Optional;

namespace DAT.EventBus
{
    public abstract class AbstractEventBus : IEventBus
    {
        public void Publish<T>(string eventName, T @event)
        {
            InternalPublish(eventName, @event);
        }

        public Option<T> Get<T>(string eventName)
        {
            return InternalGet<T>(eventName);
        }

        public IObservable<T> Subscribe<T>(string eventName)
        {
            return InternalSubscribe<T>(eventName);
        }

        public void Subscribe<T>(string eventName, Func<T, bool> handler)
        {
            InternalSubscribe<T>(eventName, handler);
        }

        protected abstract Option<T> InternalGet<T>(string eventName);

        protected abstract IObservable<T> InternalSubscribe<T>(string eventName);

        protected abstract void InternalSubscribe<T>(string eventName, Func<T, bool> handler);

        protected abstract void InternalPublish<T>(string eventName,T @event);
    }
}