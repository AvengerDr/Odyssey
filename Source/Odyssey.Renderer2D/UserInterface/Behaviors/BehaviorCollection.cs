using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Odyssey.UserInterface.Behaviors
{
    public class BehaviorCollection
    {
        private readonly List<Behavior> behaviors;
        private UIElement associatedElement;

        public BehaviorCollection()
        {
            behaviors = new List<Behavior>();
        }

        public void Add(Behavior behavior)
        {
            Contract.Requires<ArgumentNullException>(behavior != null, "behavior");
            behaviors.Add(behavior);
        }

        public void Attach(UIElement element)
        {
            Contract.Requires<ArgumentNullException>(element != null, "element");
            associatedElement = element;
            foreach (var behavior in behaviors)
                behavior.Attach(element);
        }
    }
}
