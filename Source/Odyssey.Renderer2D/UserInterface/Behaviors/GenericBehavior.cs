using System;

namespace Odyssey.UserInterface.Behaviors
{
    public abstract class Behavior<TElement> : Behavior
        where TElement : UIElement
    {
        protected new TElement AssociatedElement { get { return (TElement)base.AssociatedElement; } }
        protected Behavior() : base(typeof(TElement)) {}
    }
}
