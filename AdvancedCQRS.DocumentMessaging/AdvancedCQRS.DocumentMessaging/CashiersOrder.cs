using System;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class Cashier : IHandle<TakePayment>
    {
        private readonly IPublisher _orderHandler;

        public Cashier(IPublisher orderHandler)
        {
            _orderHandler = orderHandler;
        }

        public void Handle(TakePayment @event)
        {
            var order = new CashiersOrder(@event.Order);
            order.IsPaid = true;

            _orderHandler.Publish(new OrderPaid(order.InnerItem, @event));
        }
    }

    public class CashiersOrder
    {
        private readonly JObject _order;

        public CashiersOrder(JObject order)
        {
            _order = (JObject)order.DeepClone();
        }

        public JObject InnerItem
        {
            get { return _order; }
        }

        public bool IsPaid
        {
            get { return (bool)_order["IsPaid"]; }
            set { _order["IsPaid"] = value; }
        }
    }


}