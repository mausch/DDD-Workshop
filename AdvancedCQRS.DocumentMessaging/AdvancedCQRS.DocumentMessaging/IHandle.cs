﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCQRS.DocumentMessaging
{
    interface IHandle
    {
        void Handle(MessageBase @event);
    }

    interface IHandle<T>
    {
        void Handle(T @event);
    }
}
