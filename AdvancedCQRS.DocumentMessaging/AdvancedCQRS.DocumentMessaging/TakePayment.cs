using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class TakePayment: MessageBase
    {
        public JObject Order { get; }
        public TakePayment(JObject Order, MessageBase cause): base(cause.CorrelationId, cause.CauseId)
        {
            this.Order = Order;
        }

    }
}
