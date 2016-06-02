using Newtonsoft.Json.Linq;
using System;

namespace AdvancedCQRS.DocumentMessaging
{
    interface IPublisher
    {
        void Publish(string topic, JObject order);
    }
}
