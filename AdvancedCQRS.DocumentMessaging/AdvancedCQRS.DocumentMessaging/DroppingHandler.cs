using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCQRS.DocumentMessaging
{
    class DroppingHandler<T> : IHandle<T>
    {
        readonly IHandle<T> handler;
        readonly Random rnd = new Random();

        public DroppingHandler(IHandle<T> handler)
        {
            this.handler = handler;
        }

        public void Handle(T @event)
        {
            if (rnd.Next(0, 10) >= 5)
            {
                Console.WriteLine("Dropped " + @event);
                return;
            }
            handler.Handle(@event);
        }
    }

    static class DroppingHandler
    {
        public static DroppingHandler<T> Create<T>(IHandle<T> h)
        {
            return new DroppingHandler<T>(h);
        }
    }
}
