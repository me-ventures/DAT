
using System;

namespace DAT.EventBus
{
    public interface IEventBus
    {
        void Publish<T>(string eventName, T @event);

        T Get<T>(string eventName);
    
        IObservable<T> Subscribe<T>(string eventName);
    }
}