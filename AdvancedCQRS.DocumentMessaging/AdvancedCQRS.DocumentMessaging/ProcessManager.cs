using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class ProcessManager : IProcessManager
    {
        readonly IPublisher publisher;
        readonly ProcessManagerFactory factory;

        public ProcessManager(IPublisher publisher, ProcessManagerFactory factory)
        {
            this.publisher = publisher;
            this.factory = factory;
        }

        public void Handle(OrderPaid @event)
        {
            Console.WriteLine("Paid " + @event.CorrelationId);
            factory.Remove(@event.CorrelationId);
        }

        public void Handle(OrderPriced @event)
        {
            publisher.Publish(new TakePayment(@event.Order, @event));
        }

        public void Handle(FoodCooked @event)
        {
            publisher.Publish(new PriceOrder(@event.Order, @event));
        }

        public void Handle(OrderPlaced @event)
        {
            publisher.Publish(new CookFood(@event.Order, @event));
        }
    }

}
