using System;
using System.Diagnostics.Contracts;
using Odyssey.Utilities.Reflection;

namespace Odyssey.UserInterface.Behaviors
{
    public abstract class Behavior
    {
        private readonly Type associatedType;
        private UIElement associatedElement;
        protected UIElement AssociatedElement { get { return associatedElement; }}
        protected Type AssociatedType { get { return associatedType; } }

        protected Behavior(Type associatedType)
        {
            this.associatedType = associatedType;
        }

        public void Attach(UIElement element)
        {
            Contract.Requires<ArgumentNullException>(element!=null, "element");
            Type elementType = element.GetType();
            if (!ReflectionHelper.IsTypeDerived(elementType, AssociatedType))
                throw new ArgumentException(string.Format("Cannot attach this behaviour to an object of type '{0}'", elementType));

            associatedElement = element;
            OnAttached();
        }

        protected abstract void OnAttached();
    }
}
