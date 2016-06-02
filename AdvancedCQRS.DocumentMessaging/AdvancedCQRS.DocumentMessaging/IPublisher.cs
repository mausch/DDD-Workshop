using Newtonsoft.Json.Linq;
using System;

namespace AdvancedCQRS.DocumentMessaging
{
    interface IPublisher
    {
        void Publish<T>(T @event) where T: MessageBase;
    }
}
