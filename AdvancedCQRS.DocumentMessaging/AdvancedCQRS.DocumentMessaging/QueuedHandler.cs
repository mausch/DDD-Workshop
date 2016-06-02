using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Threading;

namespace AdvancedCQRS.DocumentMessaging
{
    class QueuedHandler : IHandleOrder, IStartable
    {
        private readonly ConcurrentQueue<JObject> messageQueue = new ConcurrentQueue<JObject>();
        private readonly IHandleOrder handler;

        public QueuedHandler(IHandleOrder handler)
        {
            this.handler = handler;
        }

        public void Handle(JObject order)
        {
            messageQueue.Enqueue(order);
        }

        public void Start()
        {
            new Thread(() => {
                while (true)
                {
                    JObject msg = null;
                    if (!messageQueue.TryDequeue(out msg)) {
                        continue;
                    }
                    handler.Handle(msg);
                    Thread.Sleep(1);
                }
            }).Start();
        }
    }
}
