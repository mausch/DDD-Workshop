using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class Manager : IHandle<PriceOrder>
    {
        private readonly IPublisher _orderHandler;

        public Manager(IPublisher orderHandler)
        {
            _orderHandler = orderHandler;
        }

        public void Handle(PriceOrder @event)
        {
            var order = new ManagersOrder(@event.Order);

            var totalWithoutTax = order.Items.Sum(item => item.Quantity * item.Price);
            var tax = (int)(totalWithoutTax * 0.2);

            order.Tax = tax;
            order.Total = totalWithoutTax + tax;

            _orderHandler.Publish(new OrderPriced(order.InnerItem));
        }
    }

    public class ManagersOrder
    {
        private readonly JObject _order;

        public ManagersOrder(JObject order)
        {
            _order = (JObject)order.DeepClone();
        }

        public JObject InnerItem
        {
            get { return _order; }
        }

        public int Tax
        {
            get { return (int)_order["Tax"]; }
            set { _order["Tax"] = value; }
        }

        public int Total
        {
            get { return (int)_order["Total"]; }
            set { _order["Total"] = value; }
        }

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
    }
}