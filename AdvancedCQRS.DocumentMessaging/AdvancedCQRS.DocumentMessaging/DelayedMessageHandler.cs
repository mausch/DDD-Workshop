using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AdvancedCQRS.DocumentMessaging
{
    class DelayedMessageHandler<T> : IHandle<DelayedMessage<T>>, IStartable where T: MessageBase
    {
        readonly IPublisher publisher;
        IList<KeyValuePair<DateTime, T>> pendingMessages = new List<KeyValuePair<DateTime, T>>();

        public DelayedMessageHandler(IPublisher publisher)
        {
            this.publisher = publisher;
        }

        public void Handle(DelayedMessage<T> @event)
        {
            pendingMessages.Add(new KeyValuePair<DateTime, T>(DateTime.Now.Add(@event.Wait), @event.Message));
            //Thread.Sleep(@event.Wait);

            //publisher.Publish(@event.Message);
        }

        public void Start()
        {
            var timer = new System.Timers.Timer(500);
            timer.Elapsed += (sender, e) => {
                var toSend = pendingMessages.Where(kv => kv.Key < DateTime.Now).ToList();
                foreach (var msg in toSend)
                {
                    Console.WriteLine("publishing delayed " + msg.Value);
                    publisher.Publish(msg.Value);
                }
                pendingMessages = pendingMessages.Except(toSend).ToList();
            };
            timer.Start();
        }
    }
}
