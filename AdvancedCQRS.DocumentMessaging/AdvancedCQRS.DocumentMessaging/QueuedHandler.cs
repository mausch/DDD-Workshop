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
    interface IQueuedHandler
    {
        int Count { get; }
        string Name { get; }
    }

    static class QueuedHandler
    {
        public static QueuedHandler<T> Create<T>(IHandle<T> handler, string name) where T: MessageBase
        {
            return new QueuedHandler<T>(handler, name);
        }
    }

    class QueuedHandler<T> : IHandle<T>, IStartable, IQueuedHandler where T: MessageBase
    {
        private readonly ConcurrentQueue<T> messageQueue = new ConcurrentQueue<T>();
        private readonly IHandle<T> handler;
        public string Name { get; }

        public QueuedHandler(IHandle<T> handler, string name)
        {
            this.handler = handler;
            this.Name = name;
        }

        public int Count => messageQueue.Count;

        public void Start()
        {
            new Thread(() => {
                while (true)
                {
                    T msg = null;
                    if (!messageQueue.TryDequeue(out msg)) {
                        continue;
                    }
                    handler.Handle(msg);
                    Thread.Sleep(1);
                }
            }).Start();
        }

        public void Handle(T @event)
        {
            messageQueue.Enqueue(@event);
        }
    }
}
