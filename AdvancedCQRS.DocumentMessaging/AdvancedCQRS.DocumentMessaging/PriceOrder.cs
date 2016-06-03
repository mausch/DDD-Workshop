using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCQRS.DocumentMessaging
{
    class PriceOrder: MessageBase
    {
        public JObject Order { get; }
        public PriceOrder(JObject Order): base()
        {
            this.Order = Order;
        }

    }
}
