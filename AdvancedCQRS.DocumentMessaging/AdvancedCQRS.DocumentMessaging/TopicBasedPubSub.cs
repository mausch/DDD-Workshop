using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class TopicBasedPubSub: IPublisher
    {
        readonly Dictionary<Type, object> handlers = new Dictionary<Type, object>();

        public void Publish<TMessage>(TMessage @event) where TMessage : MessageBase
        {
            object handler;
            if (!handlers.TryGetValue(@event.GetType(), out handler))
                return;
            var typedhandler = handler as IHandle<TMessage>;
            typedhandler.Handle(@event);
        }

        public void Subscribe<TMessage>(IHandle<TMessage> handler) where TMessage: MessageBase
        {
            handlers[typeof(TMessage)] = handler;
        }

    }
}
