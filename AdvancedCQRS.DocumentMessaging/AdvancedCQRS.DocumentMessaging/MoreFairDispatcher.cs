using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AdvancedCQRS.DocumentMessaging
{
    static class MoreFairDispatcher
    {
        public static MoreFairDispatcher<T> Create<T>(IEnumerable<QueuedHandler<T>> handlers) where T: MessageBase
        {
            return new MoreFairDispatcher<T>(handlers);
        }
    }

    class MoreFairDispatcher<T> : IHandle<T> where T: MessageBase
    {
        private readonly IEnumerable<QueuedHandler<T>> handlers;

        public MoreFairDispatcher(IEnumerable<QueuedHandler<T>> handlers)
        {
            this.handlers = handlers;
        }

        public void Handle(T @event)
        {
            while (true)
            {
                var handler = handlers.FirstOrDefault(x => x.Count < 5);
                if (handler != null)
                {
                    handler.Handle(@event);
                    return;
                } else
                {
                    Thread.Sleep(10);
                }
            }
        }
    }
}
