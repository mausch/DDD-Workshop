using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Cashier
    {
        public static JObject Handle(JObject baseOrder)
        {
            var order = new CashiersOrder(baseOrder);
            order.IsPaid = true;

            return order.InnerItem;
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