using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class DelayedMessage<T> : MessageBase where T: MessageBase
    {
        public T Message { get; }
        public TimeSpan Wait { get; }

        public DelayedMessage(T Message, TimeSpan wait, Guid CorrelationId, Guid? CauseId) : base(CorrelationId, CauseId)
        {
            this.Message = Message;
            this.Wait = wait;
        }
    }
}
