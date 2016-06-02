using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Program
    {
        public static void Main()
        {
            var startables = new List<IStartable>();
            var cashier = new QueuedHandler(new Cashier(new PrintingOrderHandler()));
            startables.Add(cashier);
            var manager = new QueuedHandler(new Manager(cashier));
            startables.Add(manager);
            var cook = new Cook(manager);
            var cooks =
                (from q in Enumerable.Repeat(cook, 3)
                select new QueuedHandler(cook)).ToList();
            startables.AddRange(cooks);
            var rrCooks = new RoundRobinDispatcher(cooks);
            var waiter = new Waiter(rrCooks);

            for (int i = 1; i < 11; i++)
            {
                waiter.TakeOrder(i, CreateOrder());
            }

            foreach (var c in startables)
                c.Start();

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