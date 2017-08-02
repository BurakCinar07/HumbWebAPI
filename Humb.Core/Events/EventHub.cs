using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Humb.Core.Events
{
    //TODO : Implement Multithreading due to usage of extarnal services.
    public static class EventHub
    {
        private static readonly Dictionary<Type, List<Subscriber>> handlers = new Dictionary<Type, List<Subscriber>>();
        private static readonly object locker = new object();
        public static void Subscribe<T>(Action<T> handler)
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                handlers[typeof(T)] = new List<Subscriber>();
            }

            handlers[typeof(T)].Add(new Subscriber(handler));
        }
        public static void Publish<T>(T eventData)
        {
            var type = typeof(T);
            if (handlers.ContainsKey(type))
            {
                var subscribers = handlers[type].ToList();

                for (int i = subscribers.Count - 1; i >= 0; i--)
                {
                    var subscriber = subscribers[i];             

                    if (InvokeSubscriberHandler(subscriber, eventData) == false)
                        subscribers.RemoveAt(i);
                }
            }
        }
        private static bool InvokeSubscriberHandler<T>(Subscriber subscriber, T eventData)
        {
            var handler = subscriber.CreateAction<T>();
            if (handler == null)
                return false;

            handler(eventData);
            return true;
        }
        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (handlers.ContainsKey(typeof(T)))
            {
                List<Subscriber> _subscribers = handlers[typeof(T)];
                _subscribers.RemoveAll(x => x._methodHandler.Equals(handler));
            }
        }

    }
}
