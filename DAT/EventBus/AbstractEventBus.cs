using System;

namespace DAT.EventBus
{
    public abstract class AbstractEventBus : IEventBus
    {
        public void Publish<T>(string eventName, T @event)
        {
            InternalPublish(eventName, @event);
        }

        public T Get<T>(string eventName)
        {
            throw new NotImplementedException();
        }

        public IObservable<T> Subscribe<T>(string eventName)
        {
            throw new NotImplementedException();
        }

        protected abstract T InternalGet<T>(string eventName);

        protected abstract T InternalSubscribe<T>(string eventName);

        protected abstract void InternalPublish<T>(string eventName,T @event);
    }
}