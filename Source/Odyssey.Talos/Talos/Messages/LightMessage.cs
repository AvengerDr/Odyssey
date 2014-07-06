﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Talos.Nodes;

namespace Odyssey.Talos.Messages
{
    public class LightMessage: EntityChangeMessage
    {
        public LightNode Light { get; private set; }

        public LightMessage(IEntity source, LightNode light, ChangeType action, bool isSynchronous = false)
            : base(source, action, isSynchronous)
        {
            Light = light;
        }
    }
}
