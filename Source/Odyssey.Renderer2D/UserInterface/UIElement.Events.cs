#region License

// Copyright © 2013-2014 Avengers UTD - Adalberto L. Simeone
//
// The Odyssey Engine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License Version 3 as published by
// the Free Software Foundation.
//
// The Odyssey Engine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details at http://gplv3.fsf.org/

#endregion License

#region Using Directives

using System.Globalization;
using Odyssey.Engine;
using Odyssey.Interaction;
using Odyssey.UserInterface.Controls;
using System;
using Odyssey.UserInterface.Serialization;
using Odyssey.Utilities.Text;
using SharpDX;

#endregion Using Directives

namespace Odyssey.UserInterface
{
    public abstract partial class UIElement
    {
        #region PointerEvents

        /// <summary>
        /// Occurs when the mouse pointer enters the control.
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerEntered;

        /// <summary>
        /// Occurs when the mouse pointer leaves the control.
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerExited;

        /// <summary>
        /// Occurs when the mouse pointer is moved over the control.
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerMoved;

        /// <summary>
        /// Occurs when mouse pointer is over the control and a mouse button is pressed..
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerPressed;

        /// <summary>
        /// Occurs when the mouse pointer is over the control and a mouse button is released.
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerReleased;

        /// <summary>
        /// Occurs when the mouse wheel moves while the control has focus.
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerWheelChanged;

        /// <summary>
        /// Occurs when the control is clicked by the mouse.
        /// </summary>
        public event EventHandler<PointerEventArgs> Tap;

        /// <summary>
        /// Raises the <see cref="PointerExited"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnPointerExited(PointerEventArgs e)
        {
            isInside = false;
            if (CanRaiseEvents)
            {
                if (e.CurrentPoint.IsLeftButtonPressed)
                {
                    //if (UserInterfaceManager.CurrentOverlay.PressedControl == this)
                    //{
                    //    IsHighlighted = IsPressed = false;
                    //}
                    //else
                    //    return;
                }
                else
                {
                    IsHighlighted = false;
                }
            }

            RaiseEvent(PointerExited, this, e);
        }

        /// <summary>
        /// Raises the <see cref="PointerEntered"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnPointerEnter(PointerEventArgs e)
        {
            isInside = true;

            if (Overlay.EnteredElement.IsInside)
                Overlay.EnteredElement.OnPointerExited(e);

            Overlay.EnteredElement = this;

            if (CanRaiseEvents)
            {
                if (e.CurrentPoint.IsLeftButtonPressed)
                {
                    //if (UserInterfaceManager.CurrentOverlay.PressedControl == this)
                    //{
                    //    IsHighlighted = IsPressed = true;
                    //}
                    //else
                    //    return;
                }
                else
                {
                    IsHighlighted = true;
                }
            }

            RaiseEvent(PointerEntered, this, e);
        }

        /// <summary>
        /// Raises the <see cref="PointerMoved"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnPointerMoved(PointerEventArgs e)
        {
            RaiseEvent(PointerMoved, this, e);
        }

        /// <summary>
        /// Raises the <see cref="PointerPressed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnPointerPressed(PointerEventArgs e)
        {
            if (Overlay.FocusedElement != this)
                Overlay.FocusedElement.OnLostFocus(EventArgs.Empty);
            
            RaiseEvent(PointerPressed, this, e);
        }

        /// <summary>
        /// Raises the <see cref="PointerReleased"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnPointerReleased(PointerEventArgs e)
        {
            //UserInterfaceManager.CurrentOverlay.PressedControl = null;
            RaiseEvent(PointerReleased, this, e);
        }

        /// <summary>
        /// Raises the <see cref="PointerWheelChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnPointerWheelChanged(PointerEventArgs e)
        {
            RaiseEvent(PointerWheelChanged, this, e);
        }

        /// <summary>
        /// Raises the <see cref="Tap"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PointerEventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnTap(PointerEventArgs e)
        {
            RaiseEvent(Tap, this, e);
        }

        #endregion PointerEvents

        #region Keyboard Events

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyDown;

        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        //public event KeyPressEventHandler KeyPress;
        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyUp;

        /// <summary>
        /// Raises the <see cref="KeyDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            if (KeyDown != null)
                KeyDown(this, e);
        }

        /// <summary>
        /// Raises the <see cref="KeyPress"/> event.
        /// </summary>
        /// <param name="e">The <see cref=".KeyPressEventArgs"/> instance containing the event data.</param>
        //protected virtual void OnKeyPress(KeyPressEventArgs e)
        //{
        //    KeyPressEventHandler handler = (KeyPressEventHandler) Events[EventKeyPress];
        //    if (handler != null)
        //        handler(this, e);
        //}
        /// <summary>
        /// Raises the <see cref="KeyUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            if (KeyUp != null)
                KeyUp(this, e);
        }

        #endregion Keyboard Events

        #region Control Events

        #region Events declaration

        /// <summary>
        /// Occurs when the control receives focus.
        /// </summary>
        public event EventHandler<EventArgs> GotFocus;

        /// <summary>
        /// Occurs when the <see cref="IsHighlighted"/> property value changes.
        /// </summary>
        public event EventHandler<EventArgs> HighlightedChanged;

        /// <summary>
        /// Occurs when the control loses focus.
        /// </summary>
        public event EventHandler<EventArgs> LostFocus;

        /// <summary>
        /// Occurs when the control is moved.
        /// </summary>
        public event EventHandler<EventArgs> Move;

        /// <summary>
        /// Occurs when the control loses mouse capture.
        /// </summary>
        public event EventHandler<PointerEventArgs> PointerCaptureChanged;

        /// <summary>
        /// Occurs when the <see cref="Position"/> property value changes.
        /// </summary>
        public event EventHandler<EventArgs> PositionChanged;

        /// <summary>
        /// Occurs when <see cref="Size"/> property value changes.
        /// </summary>
        public event EventHandler<EventArgs> SizeChanged;

        /// <summary>
        /// Occurs when the <see cref="IsVisible"/> property value changes.
        /// </summary>
        public event EventHandler<EventArgs> VisibleChanged;

        /// <summary>
        /// Occurs when the <see cref="DesignMode"/> property value changes.
        /// </summary>
        public event EventHandler<EventArgs> DesignModeChanged;

        public event EventHandler<EventArgs> Initialized;

        public event EventHandler<EventArgs> Initializing;

        public event EventHandler<EventArgs> DataContextChanged;

        public event EventHandler<TimeEventArgs> Tick;

        /// <summary>
        /// Occurs when the control's layout changes.
        /// </summary>
        public event EventHandler<EventArgs> LayoutUpdated;

        /// <summary>
        /// Occurs when the <see cref="Parent"/> property value changes.
        /// </summary>
        public event EventHandler<EventArgs> ParentChanged;

        /// <summary>
        /// Occurs when the <see cref="IsSelected"/> property value changes.
        /// </summary>
        protected event EventHandler<EventArgs> SelectedChanged;

        #endregion Events declaration

        /// <summary>
        /// Raises the <see cref="PointerCaptureChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected internal virtual void OnPointerCaptureChanged(PointerEventArgs e)
        {
            //UserInterfaceManager.CurrentOverlay.CaptureControl = null;
            IsPressed = false;

            RaiseEvent(PointerCaptureChanged, this, e);
        }

        /// <summary>
        /// Raises the <see cref="DesignModeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        ///     data.</param>
        protected virtual void OnDesignModeChanged(EventArgs e)
        {
            RaiseEvent(DesignModeChanged, this,e );
        }

        /// <summary>
        /// Raises the <see cref="GotFocus"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnGotFocus(EventArgs e)
        {
            if (Overlay.FocusedElement != this)
            {
                Overlay.FocusedElement.OnLostFocus(EventArgs.Empty);
                Overlay.FocusedElement = this;
            }

            IsFocused = true;

            RaiseEvent(GotFocus, this, e);
        }

        protected virtual void OnTick(TimeEventArgs e)
        {
            RaiseEvent(Tick, this, e);
        }

        /// <summary>
        /// Raises the <see cref="HighlightedChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnHighlightedChanged(EventArgs e)
        {
            RaiseEvent(HighlightedChanged, this, e);
        }

        protected virtual void OnInitialized(EventArgs e)
        {
            RaiseEvent(Initialized, this, e);
        }

        protected virtual void OnInitializing(EventArgs e)
        {
            RaiseEvent(Initializing, this, e);
        }

        protected virtual void OnLayoutUpdated(EventArgs e)
        {
            RaiseEvent(LayoutUpdated, this, e);
        }

        /// <summary>
        /// Raises the <see cref="LostFocus"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnLostFocus(EventArgs e)
        {
            IsFocused = IsPressed = false;
            RaiseEvent(LostFocus, this, e);
            
        }

        /// <summary>
        /// Raises the <see cref="Move"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnMove(EventArgs e)
        {
            RaiseEvent(Move, this, e);
        }

        /// <summary>
        /// Raises the <see cref="ParentChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnParentChanged(EventArgs e)
        {
            RaiseEvent(ParentChanged, this, e);
        }

        /// <summary>
        /// Raises the <see cref="PositionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnPositionChanged(EventArgs e)
        {
            RaiseEvent(PositionChanged, this, e);
        }

        /// <summary>
        /// Raises the <see cref="SelectedChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnSelectedChanged(EventArgs e)
        {
            RaiseEvent(SelectedChanged, this, e);
        }

        /// <summary>
        /// Raises the <see cref="SizeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnSizeChanged(EventArgs e)
        {
            Layout();
            RaiseEvent(SizeChanged, this, e);
        }

        /// <summary>
        /// Raises the <see cref="VisibleChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnVisibleChanged(EventArgs e)
        {
            RaiseEvent(VisibleChanged, this, e);
        }

        protected void RaiseEvent<T>(EventHandler<T> handler, object sender, T args)
            where T : EventArgs
        {
            if (handler != null)
                handler(sender, args);
        }

        #endregion Control Events

        protected virtual void OnDataContextChanged(EventArgs e)
        {
            RaiseEvent(DataContextChanged, this, EventArgs.Empty);
        }

        protected virtual void OnReadXml(XmlDeserializationEventArgs e)
        {
            var reader = e.XmlReader;

            Name = reader.GetAttribute("Name");
            
            string sPosition = reader.GetAttribute("Position");
            Position = string.IsNullOrEmpty(sPosition) ? Vector2.Zero : Text.DecodeVector2(sPosition);

            string sWidth = reader.GetAttribute("Width");
            string sHeight = reader.GetAttribute("Height");
            Width = string.IsNullOrEmpty(sWidth) ? 0 : float.Parse(sWidth, CultureInfo.InvariantCulture);
            Height = string.IsNullOrEmpty(sHeight) ? 0 : float.Parse(sHeight, CultureInfo.InvariantCulture);
        }

        protected virtual void OnWriteXml(XmlSerializationEventArgs e)
        {
            var writer = e.XmlWriter;
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Position", Text.EncodeVector2(Position));
            writer.WriteAttributeString("Width", Width.ToString("F"));
            writer.WriteAttributeString("Height", Height.ToString("F"));
        }

    }
}