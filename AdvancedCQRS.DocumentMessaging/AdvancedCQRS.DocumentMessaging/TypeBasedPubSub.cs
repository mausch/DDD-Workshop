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
            Handle(@event.GetType().ToString(), @event);
            Handle(@event.CorrelationId.ToString(), @event);
        }

        void Handle<TMessage>(string key, TMessage @event) where TMessage: MessageBase
        {
            IReadOnlyCollection<object> handlers;
            if (!handlerMap.TryGetValue(key, out handlers))
                return;
            if (handlers.Count == 0)
                return;
            Handle(handlers, @event);
        }

        void Handle<TMessage>(IEnumerable<object> handlers, TMessage @event) where TMessage: MessageBase
        {
            var typedhandlers = handlers.Select(x => x as IHandle<TMessage>).Where(x => x != null);
            foreach (var h in typedhandlers)
                h.Handle(@event);

            var nonTypedHandlers = handlers.Select(x => x as IHandle).Where(x => x != null);
            foreach (var h in nonTypedHandlers)
                h.Handle(@event);
        }

        readonly object subscriptionLock = new object();

        void Subscribe(string key, object handler)
        {
            lock (subscriptionLock)
            {
                IReadOnlyCollection<object> handlers;
                handlerMap.TryGetValue(key, out handlers);
                var newHandlers = handlers?.ToList() ?? new List<object>();
                newHandlers.Add(handler);
                handlerMap[key] = newHandlers;
            }
        }

        public void Subscribe<TMessage>(IHandle<TMessage> handler) where TMessage: MessageBase
        {
            Subscribe(typeof(TMessage).ToString(), handler);
        }

        public void SubscribeByCorrelationId<TMessage>(Guid correlationId, IHandle<TMessage> handler) where TMessage: MessageBase
        {
            Subscribe(correlationId.ToString(), handler);
        }

        public void SubscribeByCorrelationId(Guid correlationId, IHandle handler)
        {
            Subscribe(correlationId.ToString(), handler);
        }

        public void UnsubcribeByCorrelationId(Guid correlationId, IHandle handler)
        {
            Unsubscribe(correlationId.ToString(), handler);
        }

        void Unsubscribe(string key, object handler)
        {
            lock (subscriptionLock)
            {
                IReadOnlyCollection<object> handlers;
                handlerMap.TryGetValue(key, out handlers);
                var newHandlers = handlers ?? Enumerable.Empty<object>();
                newHandlers = newHandlers.Where(x => x != handler);
                handlerMap[key] = newHandlers.ToList();
            }
        }

        public void Unsubscribe<TMessage>(IHandle<TMessage> handler) where TMessage: MessageBase
        {
            Unsubscribe(typeof(TMessage).ToString(), handler);
        }

    }
}
