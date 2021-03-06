﻿using Odyssey.Interaction;
using SharpDX.Mathematics;

namespace Odyssey.UserInterface.Behaviors
{
    public class DraggableBehavior : Behavior<UIElement>
    {
        private Vector3 offset;

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
                offset = new Vector3(e.CurrentPoint.Position, 0) - AssociatedElement.AbsolutePosition;
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
