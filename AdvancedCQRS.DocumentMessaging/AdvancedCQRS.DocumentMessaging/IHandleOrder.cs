using System;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public interface IHandleOrder
    {
        void Handle(JObject order);
    }

    class OrderHandler : IHandleOrder
    {
        readonly Action<JObject> handler;

        public OrderHandler(Action<JObject> handler)
        {
            this.handler = handler;
        }

        public void Handle(JObject order)
        {
            handler(order);
        }
    }
}