using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Interaction;
using SharpDX;

namespace Odyssey.UserInterface.Behaviors
{
    public class DraggableBehavior : Behavior<UIElement>
    {
        private Vector2 offset;

        protected override void OnAttached()
        {
            AssociatedElement.PointerPressed += OnPointerPressed;
            AssociatedElement.PointerReleased += OnPointerReleased;
            AssociatedElement.PointerMoved += OnPointerMoved;

        }

        void OnPointerPressed(object sender, PointerEventArgs e)
        {
            if (e.CurrentPoint.IsLeftButtonPressed)
            {
                offset = e.CurrentPoint.Position - AssociatedElement.AbsolutePosition;
                AssociatedElement.CapturePointer();
            }
        }

        void OnPointerMoved(object sender, PointerEventArgs e)
        {
            if (AssociatedElement.IsPointerCaptured)
                AssociatedElement.Position = UIElement.ScreenToLocalCoordinates(AssociatedElement, e.CurrentPoint.Position) - offset;
        }

        void OnPointerReleased(object sender, PointerEventArgs e)
        {
            if (!e.CurrentPoint.IsLeftButtonPressed)
                AssociatedElement.ReleaseCapture();
        }
    }
}
