using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class TopicBasedPubSub: IPublisher
    {
        readonly Dictionary<string, IHandleOrder> handlers = new Dictionary<string, IHandleOrder>();

        public void Publish(string topic, JObject order)
        {
            IHandleOrder handler;
            if (!handlers.TryGetValue(topic, out handler))
                return;
            handler.Handle(order);
        }

        public void Subscribe(string topic, IHandleOrder handler)
        {
            handlers[topic] = handler;
        }

    }
}
