using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Program
    {
        static readonly Func<JObject, JObject> pipeline = 
            new Func<JObject, JObject>[] {
                Cook.Handle,
                Manager.Handle,
                Cashier.Handle,
                Printer,
            }.Aggregate((f,g) => x => g(f(x)));

        static JObject TakeOrder(int table, IEnumerable<LineItem> items)
        {
            return pipeline(Waiter.TakeOrder(table, items).Item2);
        }

        public static void Main()
        {
            //for (int i = 1; i < 11; i++)
            var i = 0;
            {
                TakeOrder(i, CreateOrder());
            }
        }

        static T Printer<T>(T o)
        {
            Console.WriteLine(o);
            return o;
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