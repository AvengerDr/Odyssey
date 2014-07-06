﻿using Odyssey.Talos.Components;
using System.Diagnostics;

namespace Odyssey.Talos.Messages
{
    [DebuggerDisplay("Property = {Property}")]
    public class PropertyChangeMessage : Message
    {
        public Component Component { get; private set; }
        public string Property { get; private set; }

        public PropertyChangeMessage(string property, Component component, bool isSynchronous = false)
            : base(isSynchronous)
        {
            Property = property;
            Component = component;
        }
    }
}
