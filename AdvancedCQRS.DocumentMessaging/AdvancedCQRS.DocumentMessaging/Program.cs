using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Program
    {
        public static void Main()
        {
            var cashier = new Cashier(new PrintingOrderHandler());
            var manager = new Manager(cashier);
            var cook = new Cook(manager);
            var queuedCooks =
                (from q in Enumerable.Repeat(cook, 3)
                select new QueuedHandler(cook)).ToList();
            var cooks = new RoundRobinDispatcher(queuedCooks);
            var waiter = new Waiter(cooks);

            for (int i = 1; i < 11; i++)
            {
                waiter.TakeOrder(i, CreateOrder());
            }

            foreach (var c in queuedCooks)
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