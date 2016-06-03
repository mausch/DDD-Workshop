using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class Cook : IHandle<CookFood>
    {
        private readonly IPublisher _orderHandler;
        public string Name { get; }
        private readonly int sleepTime;

        public Cook(IPublisher orderHandler, string name, int sleepTime)
        {
            _orderHandler = orderHandler;
            this.Name = name;
            this.sleepTime = sleepTime;
        }

        private readonly Dictionary<string, string> _ingredientsMap = new Dictionary<string, string>
        {
            {"razor blade ice cream", "razor blades, ice cream" },
            {"random1", "meat, tomatos" },
            {"random2", "onions, olives" },
            {"random3", "secret" },
        };

        private string FindIngredients(LineItem item)
        {
            if (_ingredientsMap.ContainsKey(item.Item))
            {
                return _ingredientsMap[item.Item];
            }
            
            return _ingredientsMap[$"random{new Random().Next(1, 3)}"];
        }

        public void Handle(CookFood @event)
        {
            //Console.WriteLine(Name + " is cooking");
            var order = new CooksOrder(@event.Order);

            //var timeToCook = TimeToCook(string.Join(" ", order.Items.Select(x => x.Item)));
            Thread.Sleep(sleepTime);

            order.Ingredients = string.Join(", ", order.Items.Select(FindIngredients));
            order.CookedAt = DateTime.Now;

            _orderHandler.Publish(new FoodCooked(order.InnerItem, @event));
        }
    }

    public class CooksOrder
    {
        private readonly JObject _order;

        public CooksOrder(JObject order)
        {
            _order = (JObject)order.DeepClone();
        }

        public JObject InnerItem => _order;

        public IEnumerable<LineItem> Items
        {
            get
            {
                foreach (JObject item in _order["LineItems"] ?? new JArray())
                {
                    yield return new LineItem(item);
                }
            }
        }

        public DateTime CookedAt
        {
            set { _order["CookedAt"] = value; }
        }

        public string Ingredients
        {
            get { return (string)_order["Ingredients"]; }
            set { _order["Ingredients"] = value; }
        }
    }
}