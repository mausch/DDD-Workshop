using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class MessageBase
    {
        public Guid Id { get; }
        public Guid CorrelationId { get; }
        public Guid? CauseId { get; }

        //public MessageBase(MessageBase msg): this(msg.CorrelationId, msg.CauseId) { }

        public MessageBase(Guid CorrelationId, Guid? CauseId) {
            Id = Guid.NewGuid();
            this.CorrelationId = CorrelationId;
            this.CauseId = CauseId;
        }
    }
}