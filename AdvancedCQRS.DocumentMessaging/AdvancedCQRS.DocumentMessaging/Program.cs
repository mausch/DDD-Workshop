﻿using System.Collections.Generic;
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
            var queues = new List<QueuedHandler>();

            var pubsub = new TopicBasedPubSub();

            var cashier = new QueuedHandler(new Cashier(pubsub), "cashier");
            startables.Add(cashier);
            queues.Add(cashier);
            pubsub.Subscribe("priced", cashier);

            var manager = new QueuedHandler(new Manager(pubsub), "manager");
            startables.Add(manager);
            queues.Add(manager);
            pubsub.Subscribe("cooked", manager);

            var rnd = new Random();
            var cooks = new[] {
                new Cook(pubsub, "Tom", rnd.Next(10, 1000)),
                new Cook(pubsub, "Jane", rnd.Next(10, 1000)),
                new Cook(pubsub, "Dan", rnd.Next(10, 1000)),
            }.Select(x => new QueuedHandler(x, x.Name)).ToList();
            startables.AddRange(cooks);
            queues.AddRange(cooks);

            var rrCooks = new QueuedHandler(new MoreFairDispatcher(cooks), "fair dispatcher");
            startables.Add(rrCooks);
            queues.Add(rrCooks);
            pubsub.Subscribe("cook", rrCooks);

            var waiter = new Waiter(pubsub);

            for (int i = 1; i < 300; i++)
            {
                waiter.TakeOrder(i, CreateOrder());
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