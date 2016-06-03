using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCQRS.DocumentMessaging
{
    class ProcessManager2 : IProcessManager
    {
        readonly IPublisher publisher;
        readonly ProcessManagerFactory factory;

        public ProcessManager2(IPublisher publisher, ProcessManagerFactory factory)
        {
            this.publisher = publisher;
            this.factory = factory;
        }

        public void Handle(OrderPaid @event)
        {
            publisher.Publish(new CookFood(@event.Order, @event));
        }

        public void Handle(OrderPriced @event)
        {
            publisher.Publish(new TakePayment(@event.Order, @event));
        }

        public void Handle(FoodCooked @event)
        {
            Console.WriteLine("Cooked " + @event.CorrelationId);
            factory.Remove(@event.CorrelationId);
        }

        public void Handle(OrderPlaced @event)
        {
            publisher.Publish(new PriceOrder(@event.Order, @event));
        }
    }
}
