using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace AdvancedCQRS.DocumentMessaging
{
    class MoreFairDispatcher : IHandleOrder
    {
        private readonly IEnumerable<QueuedHandler> handlers;

        public MoreFairDispatcher(IEnumerable<QueuedHandler> handlers)
        {
            this.handlers = handlers;
        }

        public void Handle(JObject order)
        {
            while (true)
            {
                var handler = handlers.FirstOrDefault(x => x.Count < 5);
                if (handler != null)
                {
                    handler.Handle(order);
                    return;
                } else
                {
                    Thread.Sleep(10);
                }
            }
        }
    }
}
