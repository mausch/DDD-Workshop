using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCQRS.DocumentMessaging
{
    interface IProcessManager: IHandle<OrderPlaced>, IHandle<FoodCooked>, IHandle<OrderPriced>, IHandle<OrderPaid>
    {
    }
}
