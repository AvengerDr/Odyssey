﻿using Odyssey.Talos.Systems;
using Odyssey.Utilities.Collections;

namespace Odyssey.Talos.Messages
{
    public class MessageQueue : QueueMap<IMessage>
    {
        public MessageQueue(ISystem systemOwner)
        {
        }

    }
}