using System;
using Optional;

namespace DAT.EventBus
{
    public interface IEventBus
    {
        /// <summary>
        /// Publish message to the eventbus. Event name is the complete name name of the event. How the name is being
        /// handled depends on the actual implementation.
        /// </summary>
        /// <param name="eventName">Name of the eventbus</param>
        /// <param name="event"></param>
        /// <typeparam name="T"></typeparam>
        void Publish<T>(string eventName, T @event);

        /// <summary>
        /// Retrieve one message, if no message is present None will be returned. This will not wait until a message is
        /// retrieved.
        /// </summary>
        /// <param name="eventName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Option<T> Get<T>(string eventName);
    
        /// <summary>
        /// Subscribe to a message stream one particular stream. Currently not able to reject messages.
        /// </summary>
        /// <param name="eventName">Name of the eventbus</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IObservable<T> Subscribe<T>(string eventName);

        /// <summary>
        /// Subscribe to a message stream one particular stream. Handler will be called for every message that is
        /// retrieved. If the return value of the handler is false, the message will be rejected.
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handler"></param>
        /// <typeparam name="T"></typeparam>
        void Subscribe<T>(string eventName, Func<T, bool> handler);
    }
}