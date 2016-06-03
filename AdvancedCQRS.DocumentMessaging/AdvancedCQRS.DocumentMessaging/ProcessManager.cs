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

        bool isCooked = false;

        public void Handle(FoodCooked @event)
        {
            isCooked = true;
            publisher.Publish(new PriceOrder(@event.Order, @event));
        }

        public void Handle(OrderPlaced @event)
        {
            var cookfood = new CookFood(@event.Order, @event);
            publisher.Publish(cookfood);
            publisher.Publish(new DelayedMessage<CookFood>(cookfood, TimeSpan.FromSeconds(1), cookfood.CorrelationId, cookfood.Id));
        }

        public void Handle(RetryCookFood @event)
        {
            if (isCooked)
                return;
            Console.WriteLine("retrying cooking");
            publisher.Publish(@event.Message);
        }
    }

}
