#region Using Directives

using Odyssey.Content;
using Odyssey.Interaction;
using SharpDX;
using System;

#endregion Using Directives

namespace Odyssey.UserInterface
{
    public interface IUIElement : IResource
    {
        Vector2 AbsolutePosition { get; }

        bool CanRaiseEvents { get; set; }

        bool IsEnabled { get; set; }

        bool IsFocused { get; }

        bool IsHighlighted { get; set; }

        bool IsVisible { get; set; }

        UIElement Parent { get; }

        Vector2 Position { get; set; }

        event EventHandler<EventArgs> GotFocus;

        event EventHandler<EventArgs> HighlightedChanged;

        event EventHandler<KeyEventArgs> KeyDown;

        //event KeyPressEventHandler KeyPress;
        event EventHandler<KeyEventArgs> KeyUp;

        event EventHandler<EventArgs> LostFocus;

        event EventHandler<EventArgs> Move;

        event EventHandler<PointerEventArgs> PointerCaptureChanged;

        event EventHandler<PointerEventArgs> PointerEntered;

        event EventHandler<PointerEventArgs> PointerExited;

        event EventHandler<PointerEventArgs> PointerMoved;

        event EventHandler<PointerEventArgs> PointerPressed;

        event EventHandler<PointerEventArgs> PointerReleased;

        event EventHandler<PointerEventArgs> PointerWheelChanged;

        event EventHandler<EventArgs> PositionChanged;

        event EventHandler<EventArgs> SizeChanged;

        event EventHandler<PointerEventArgs> Tap;

        event EventHandler<EventArgs> VisibleChanged;

        bool Contains(Vector2 cursorLocation);

        void Focus();

        void Render();
    }
}