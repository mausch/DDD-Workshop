using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class ProcessManager : IHandle<OrderPlaced>, IHandle<FoodCooked>, IHandle<OrderPriced>, IHandle<OrderPaid>
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
            factory.Handle(@event);
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

    class ProcessManagerFactory : IHandle<OrderPlaced>
    {
        readonly Dictionary<Guid, ProcessManager> procManagers = new Dictionary<Guid, ProcessManager>();
        readonly TypeBasedPubSub pubsub;

        public ProcessManagerFactory(TypeBasedPubSub pubsub)
        {
            this.pubsub = pubsub;
        }

        public void Handle(OrderPaid @event)
        {
            procManagers.Remove(@event.CorrelationId);
        }

        public void Handle(OrderPlaced @event)
        {
            var procManager = new ProcessManager(pubsub, this);
            procManagers[@event.CorrelationId] = procManager;
            pubsub.SubscribeByCorrelationId<FoodCooked>(@event.CorrelationId, procManager);
            pubsub.SubscribeByCorrelationId<OrderPriced>(@event.CorrelationId, procManager);
            pubsub.SubscribeByCorrelationId<OrderPaid>(@event.CorrelationId, procManager);
            procManager.Handle(@event);
        }
    }
}
