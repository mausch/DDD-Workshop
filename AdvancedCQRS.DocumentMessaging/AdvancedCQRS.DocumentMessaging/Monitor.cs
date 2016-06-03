using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCQRS.DocumentMessaging
{
    class Monitor : IHandle
    {
        public void Handle(MessageBase @event)
        {
            Console.WriteLine($"{@event.GetType().Name} id {@event.Id} correlation {@event.CorrelationId} cause {@event.CauseId}");
        }
    }
}
