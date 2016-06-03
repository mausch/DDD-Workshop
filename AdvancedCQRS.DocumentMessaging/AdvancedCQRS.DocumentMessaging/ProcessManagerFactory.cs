using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCQRS.DocumentMessaging
{
    class ProcessManagerFactory : IHandle<OrderPlaced>
    {
        readonly Dictionary<Guid, ProcessManager> procManagers = new Dictionary<Guid, ProcessManager>();
        readonly TypeBasedPubSub pubsub;

        public ProcessManagerFactory(TypeBasedPubSub pubsub)
        {
            this.pubsub = pubsub;
        }

        public void Remove(Guid id)
        {
            procManagers.Remove(id);
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
