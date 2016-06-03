using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCQRS.DocumentMessaging
{
    class ProcessManagerFactory : IHandle<OrderPlaced>
    {
        readonly Dictionary<Guid, IProcessManager> procManagers = new Dictionary<Guid, IProcessManager>();
        readonly TypeBasedPubSub pubsub;

        public ProcessManagerFactory(TypeBasedPubSub pubsub)
        {
            this.pubsub = pubsub;
        }

        public void Remove(Guid id)
        {
            procManagers.Remove(id);
        }

        IProcessManager BuildProcessManager(OrderPlaced @event)
        {
            var order = new WaitersOrder(@event.Order);
            IProcessManager procManager = new ProcessManager(pubsub, this);
            if (order.IsDodgy)
            {
                return new ProcessManager(pubsub, this);

            } else
            {
                Console.WriteLine("dodgy order!");
                return new ProcessManager2(pubsub, this);
            }
        }

        public void Handle(OrderPlaced @event)
        {
            var procManager = BuildProcessManager(@event);
            procManagers[@event.CorrelationId] = procManager;
            pubsub.SubscribeByCorrelationId<FoodCooked>(@event.CorrelationId, procManager);
            pubsub.SubscribeByCorrelationId<OrderPriced>(@event.CorrelationId, procManager);
            pubsub.SubscribeByCorrelationId<OrderPaid>(@event.CorrelationId, procManager);
            procManager.Handle(@event);
        }
    }
}
