using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class ProcessManager : IHandle<OrderPlaced>, IHandle<FoodCooked>, IHandle<OrderPriced>
    {
        readonly TypeBasedPubSub pubsub;

        public ProcessManager(TypeBasedPubSub pubsub)
        {
            this.pubsub = pubsub;
        }

        public void Handle(OrderPriced @event)
        {
            pubsub.Publish(new TakePayment(@event.Order, @event));
        }

        public void Handle(FoodCooked @event)
        {
            pubsub.Publish(new PriceOrder(@event.Order, @event));
        }

        public void Handle(OrderPlaced @event)
        {
            pubsub.Publish(new CookFood(@event.Order, @event));
        }
    }

    class ProcessManagerFactory : IHandle<OrderPlaced>, IHandle<OrderPaid>, IHandle<FoodCooked>, IHandle<OrderPriced>
    {
        readonly Dictionary<Guid, ProcessManager> procManagers = new Dictionary<Guid, ProcessManager>();
        readonly TypeBasedPubSub pubsub;

        public ProcessManagerFactory(TypeBasedPubSub pubsub)
        {
            this.pubsub = pubsub;
        }

        public void Handle(FoodCooked @event)
        {
            procManagers[@event.CorrelationId].Handle(@event);
        }

        public void Handle(OrderPriced @event)
        {
            procManagers[@event.CorrelationId].Handle(@event);
        }

        public void Handle(OrderPaid @event)
        {
            //procManagers[@event.CorrelationId].Handle(@event);
        }

        public void Handle(OrderPlaced @event)
        {
            var procManager = new ProcessManager(pubsub);
            procManagers[@event.CorrelationId] = procManager;
            //pubsub.SubscribeByCorrelationId(@event.CorrelationId, procManager);
            procManager.Handle(@event);
        }

        // TODO clean up from dictionary when done

    }
}
