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
            IReadOnlyCollection<object> handlersByType;
            handlerMap.TryGetValue(@event.GetType().ToString(), out handlersByType);
            handlersByType = handlersByType ?? new object[0];

            IReadOnlyCollection<object> handlersByCorrelationId;
            handlerMap.TryGetValue(@event.CorrelationId.ToString(), out handlersByCorrelationId);
            handlersByCorrelationId = handlersByCorrelationId ?? new object[0];

            var handlers = handlersByType.Concat(handlersByCorrelationId);
            var typedhandlers = handlers.Select(x => x as IHandle<TMessage>).Where(x => x != null);
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

        public void Unsubscribe<TMessage>(IHandle<TMessage> handler) 
            where TMessage: MessageBase
        {
            lock (subscriptionLock)
            {
                IReadOnlyCollection<object> handlers;
                handlerMap.TryGetValue(typeof(TMessage).ToString(), out handlers);
                var newHandlers = handlers ?? Enumerable.Empty<object>();
                newHandlers = newHandlers.Where(x => x != handler);
                handlerMap[typeof(TMessage).ToString()] = newHandlers.ToList();
            }
        }

    }
}
