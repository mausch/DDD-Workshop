using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class NullOrderHandler : IHandleOrder
    {
        public void Handle(JObject order)
        {
        }
    }
}
