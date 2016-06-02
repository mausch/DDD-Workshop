using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class OrderPlaced: MessageBase
    {
        public JObject Order { get; }
        public OrderPlaced(JObject Order): base()
        {
            this.Order = Order;
        }
    }
}
