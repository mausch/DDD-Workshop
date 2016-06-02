using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class Cashier : IHandleOrder
    {
        private readonly IPublisher _orderHandler;

        public Cashier(IPublisher orderHandler)
        {
            _orderHandler = orderHandler;
        }

        public void Handle(JObject baseOrder)
        {
            var order = new CashiersOrder(baseOrder);
            order.IsPaid = true;

            _orderHandler.Publish("paid", order.InnerItem);
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