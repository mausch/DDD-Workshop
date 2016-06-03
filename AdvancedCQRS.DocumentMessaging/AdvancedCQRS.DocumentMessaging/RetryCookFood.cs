using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCQRS.DocumentMessaging
{
    class RetryCookFood : MessageBase
    {
        public CookFood Message { get; }
        public RetryCookFood(CookFood Message, Guid CorrelationId, Guid? CauseId) : base(CorrelationId, CauseId)
        {
            this.Message = Message;
        }
    }
}
