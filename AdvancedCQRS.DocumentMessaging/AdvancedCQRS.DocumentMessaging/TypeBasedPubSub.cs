using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class TypeBasedPubSub: IPublisher
    {
        readonly Dictionary<string, IReadOnlyCollection<object>> handlerMap = new Dictionary<string, IReadOnlyCollection<object>>();

        public void Publish<TMessage>(TMessage @event) where TMessage : MessageBase
        {
            IReadOnlyCollection<object> handlers;
            if (!handlerMap.TryGetValue(@event.GetType().ToString(), out handlers))
                return;
            var typedhandlers = handlers.Select(x => x as IHandle<TMessage>);
            foreach (var h in typedhandlers)
                h.Handle(@event);
        }

        readonly object subscriptionLock = new object();

        public void Subscribe<TMessage>(IHandle<TMessage> handler) where TMessage: MessageBase
        {
            lock (subscriptionLock)
            {
                IReadOnlyCollection<object> handlers;
                handlerMap.TryGetValue(typeof(TMessage).ToString(), out handlers);
                var newHandlers = handlers?.ToList() ?? new List<object>();
                newHandlers.Add(handler);
                handlerMap[typeof(TMessage).ToString()] = newHandlers;
            }
        }

        public void Unsubscribe<THandler, TMessage>(THandler handler) 
            where TMessage: MessageBase
            where THandler: IHandle<TMessage>
        {
            lock (subscriptionLock)
            {
                IReadOnlyCollection<object> handlers;
                handlerMap.TryGetValue(typeof(TMessage).ToString(), out handlers);
                var newHandlers = handlers ?? Enumerable.Empty<object>();
                newHandlers = newHandlers.Where(x => !(x is THandler));
                handlerMap[typeof(TMessage).ToString()] = newHandlers.ToList();
            }
        }

    }
}
