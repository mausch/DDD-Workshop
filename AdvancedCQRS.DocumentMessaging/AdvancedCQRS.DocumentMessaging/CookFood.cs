﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCQRS.DocumentMessaging
{
    class CookFood: MessageBase
    {
        public JObject Order { get; }
        public CookFood(JObject Order): base()
        {
            this.Order = Order;
        }
    }
}
