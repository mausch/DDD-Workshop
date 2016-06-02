using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Waiter
    {
        public static Tuple<Guid, JObject> TakeOrder(int tableNumber, IEnumerable<LineItem> items)
        {
            var order = new WaitersOrder(new JObject());
            order.Id = Guid.NewGuid();
            order.TableNumber = tableNumber;
            order.OrderTakenAt = DateTime.Now;
            
            foreach (var item in items)
            {
                order.AddItem(item);
            }

            return Tuple.Create(order.Id, order.InnerItem);
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