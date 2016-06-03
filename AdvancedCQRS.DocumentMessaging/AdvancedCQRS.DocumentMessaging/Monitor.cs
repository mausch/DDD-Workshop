using System;
using System.Collections.Generic;

namespace AdvancedCQRS.DocumentMessaging
{
    class Monitor : IHandle
    {
        public void Handle(MessageBase @event)
        {
            Console.WriteLine(@event);
        }
    }
}
