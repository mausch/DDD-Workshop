using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class Multiplexor : IHandleOrder
    {
        private readonly IEnumerable<IHandleOrder> handlers;

        public Multiplexor(IEnumerable<IHandleOrder> handlers)
        {
            this.handlers = handlers;
        }

        public void Handle(JObject order)
        {
            foreach (var h in handlers)
                h.Handle(order);
        }
    }
}
