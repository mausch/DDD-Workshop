using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class TypeBasedPubSub: IPublisher
    {
        readonly Dictionary<Type, IReadOnlyCollection<object>> handlerMap = new Dictionary<Type, IReadOnlyCollection<object>>();

        public void Publish<TMessage>(TMessage @event) where TMessage : MessageBase
        {
            IReadOnlyCollection<object> handler;
            if (!handlerMap.TryGetValue(@event.GetType(), out handler))
                return;
            var typedhandlers = handler.Select(x => x as IHandle<TMessage>);
            foreach (var h in typedhandlers)
                h.Handle(@event);
        }

        readonly object subscriptionLock = new object();

        public void Subscribe<TMessage>(IHandle<TMessage> handler) where TMessage: MessageBase
        {
            lock (subscriptionLock)
            {
                IReadOnlyCollection<object> handlers;
                handlerMap.TryGetValue(typeof(TMessage), out handlers);
                var newHandlers = handlers?.ToList() ?? new List<object>();
                newHandlers.Add(handler);
                handlerMap[typeof(TMessage)] = newHandlers;
            }
        }

        public void Unsubscribe<THandler, TMessage>(THandler handler) 
            where TMessage: MessageBase
            where THandler: IHandle<TMessage>
        {
            lock (subscriptionLock)
            {
                IReadOnlyCollection<object> handlers;
                handlerMap.TryGetValue(typeof(TMessage), out handlers);
                var newHandlers = handlers ?? Enumerable.Empty<object>();
                newHandlers = newHandlers.Where(x => !(x is THandler));
                handlerMap[typeof(TMessage)] = newHandlers.ToList();
            }
        }

    }
}
