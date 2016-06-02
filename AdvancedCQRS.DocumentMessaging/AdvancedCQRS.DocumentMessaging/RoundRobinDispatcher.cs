using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class RoundRobinDispatcher : IHandleOrder
    {
        private readonly Queue<IHandleOrder> handlers;

        public RoundRobinDispatcher(IEnumerable<IHandleOrder> handlers)
        {
            this.handlers = new Queue<IHandleOrder>(handlers);
        }

        public void Handle(JObject order)
        {
            var handler = handlers.Dequeue();
            try
            {
                handler.Handle(order);
            } finally
            {
                handlers.Enqueue(handler);
            }            
        }
    }
}
