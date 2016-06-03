using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Threading;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Program
    {
        public static void Main()
        {
            var startables = new List<IStartable>();
            var queues = new List<IQueuedHandler>();

            var pubsub = new TypeBasedPubSub();

            var cashier = QueuedHandler.Create(new Cashier(pubsub), "cashier");
            startables.Add(cashier);
            queues.Add(cashier);
            pubsub.Subscribe(cashier);

            var manager = QueuedHandler.Create(new Manager(pubsub), "manager");
            startables.Add(manager);
            queues.Add(manager);
            pubsub.Subscribe(manager);

            var rnd = new Random();
            var cooks = new[] {
                new Cook(pubsub, "Tom", rnd.Next(10, 1000)),
                new Cook(pubsub, "Jane", rnd.Next(10, 1000)),
                new Cook(pubsub, "Dan", rnd.Next(10, 1000)),
            }.Select(x => QueuedHandler.Create(x, x.Name)).ToList();
            startables.AddRange(cooks);
            queues.AddRange(cooks);

            var rrCooks = QueuedHandler.Create(MoreFairDispatcher.Create(cooks), "fair dispatcher");
            startables.Add(rrCooks);
            queues.Add(rrCooks);
            pubsub.Subscribe(rrCooks);

            var waiter = new Waiter(pubsub);

            //var procManagerFactory = QueuedHandler.Create<OrderPlaced>(new ProcessManagerFactory(pubsub), "process manager factory");
            //startables.Add(procManagerFactory);
            //queues.Add(procManagerFactory);
            var procManagerFactory = new ProcessManagerFactory(pubsub);
            pubsub.Subscribe<OrderPlaced>(procManagerFactory);
            pubsub.Subscribe<FoodCooked>(procManagerFactory);
            pubsub.Subscribe<OrderPriced>(procManagerFactory);
            pubsub.Subscribe<OrderPaid>(procManagerFactory);


            for (int i = 1; i < 300; i++)
            {
                var id = waiter.TakeOrder(i, CreateOrder());
                pubsub.SubscribeByCorrelationId(id, new Monitor());
            }

            foreach (var c in startables)
                c.Start();

            var timer = new System.Timers.Timer(500);
            timer.Elapsed += (sender, e) => {
                Console.WriteLine();
                foreach (var q in queues)
                    Console.WriteLine($"{q.Name} queue size: {q.Count}");
            };
            timer.Start();
        }

        private static IEnumerable<LineItem> CreateOrder()
        {
            return new[]
            {
                new LineItem(new JObject())
                { Item = "razor blade ice cream", Quantity = 2, Price = 399 },
                new LineItem(new JObject())
                {
                    Item = "meat burger",
                    Quantity = 1,
                    Price = 550
                },
                new LineItem(new JObject())
                {
                    Item = "pizza",
                    Quantity = 1,
                    Price = 550
                },
            };
        }
    }
}