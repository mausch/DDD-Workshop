using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class Waiter
    {
        private readonly IPublisher _orderHandler;

        public Waiter(IPublisher orderHandler)
        {
            _orderHandler = orderHandler;
        }

        readonly Random rnd = new Random();

        public Guid TakeOrder(int tableNumber, IEnumerable<LineItem> items)
        {
            var order = new WaitersOrder(new JObject());
            order.Id = Guid.NewGuid();
            order.TableNumber = tableNumber;
            order.OrderTakenAt = DateTime.Now;
            order.IsDodgy = rnd.Next(0, 2) == 0;
            
            foreach (var item in items)
            {
                order.AddItem(item);
            }

            _orderHandler.Publish(new OrderPlaced(order.InnerItem, order.Id));
            //_orderHandler.Handle(order.InnerItem);

            return order.Id;
        }
    }

    public class WaitersOrder
    {
        private readonly JObject _order;

        public WaitersOrder(JObject order)
        {
            _order = (JObject)order.DeepClone();
        }

        public JObject InnerItem => _order;

        public Guid Id
        {
            get { return (Guid)_order["Id"]; }
            set { _order["Id"] = value; }
        }

        public DateTime OrderTakenAt
        {
            set { _order["OrderTakenAt"] = value; }
        }

        public int TableNumber
        {
            get { return (int)_order["TableNumber"]; }
            set { _order["TableNumber"] = value; }
        }

        public bool IsDodgy
        {
            get { return (bool)_order["IsDodgy"]; }
            set { _order["IsDodgy"] = value;  }
        }

        public void AddItem(LineItem item)
        {
            if (_order["LineItems"] == null)
            {
                _order["LineItems"] = new JArray();
            }

            ((JArray)_order["LineItems"]).Add(item.InnerItem);
        }
    }
}