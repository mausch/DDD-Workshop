using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    class MessageBase
    {
        public Guid Id { get; }
        public MessageBase() {
            Id = Guid.NewGuid();
        }
    }
}