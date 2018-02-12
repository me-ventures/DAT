using System;
using Optional;

namespace DAT.EventBus
{
    public interface IEventBus
    {
        void Publish<T>(string eventName, T @event);

        Option<T> Get<T>(string eventName);
    
        IObservable<T> Subscribe<T>(string eventName);

        void Subscribe<T>(string eventName, Func<T, bool> handler);
    }
}