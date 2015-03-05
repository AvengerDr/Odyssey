#region License

// Copyright � 2013-2014 Avengers UTD - Adalberto L. Simeone
// 
// The Odyssey Engine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License Version 3 as published by
// the Free Software Foundation.
// 
// The Odyssey Engine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details at http://gplv3.fsf.org/

#endregion

#region Using Directives

using System;
using System.Collections.Generic;
using Odyssey.Animations;
using Odyssey.Core;
using Odyssey.Engine;
using Odyssey.UserInterface.Behaviors;
using Odyssey.UserInterface.Controls;
using Odyssey.UserInterface.Data;
using Odyssey.UserInterface.Events;
using Odyssey.UserInterface.Style;
using SharpDX.Mathematics;

#endregion

namespace Odyssey.UserInterface
{
    public abstract partial class UIElement
    {
        public PropertyContainer DependencyProperties;
        private Vector3 positionOffsets;
        protected internal bool IsInternal { get; set; }

        internal Vector3 MarginInternal
        {
            get { return new Vector3(Margin.Horizontal, Margin.Vertical, 0); }
        }

        /// <summary>
        ///     Returns the publicly available collection of child controls.
        /// </summary>
        public UIElementCollection Children { get; private set; }

        public RectangleF BoundingRectangle
        {
            get { return boundingRectangle; }
        }

        public object DataContext
        {
            get { return dataContext; }
            set
            {
                if (dataContext == value)
                    return;
                dataContext = value;
                OnDataContextChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the control is in design mode.
        /// </summary>
        /// <value><c>true</c> if the <see cref="UIElement" /> is in design mode; otherwise, /c>.</value>
        /// <remarks>
        ///     While in design mode, certain events are not fired.
        /// </remarks>
        public bool DesignMode
        {
            get { return designMode; }
            protected internal set
            {
                if (designMode == value)
                    return;
                designMode = value;
                foreach (UIElement childControl in Children)
                    childControl.DesignMode = value;
            }
        }

        /// <summary>
        ///     Determines whether this control has captured the mouse pointer.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the control has captured the mouse cursor, <c>false</c>
        ///     otherwise.
        /// </value>
        /// <remarks>
        ///     When a control captures the mouse pointer, events are only sent to that control.
        /// </remarks>
        public bool IsPointerCaptured { get; internal set; }

        public float Height
        {
            get { return height; }
            set
            {
                if (height == value)
                    return;

                height = value;
                if (!DesignMode)
                    InvalidateMeasure();
            }
        }

        /// <summary>
        ///     Gets the zero based index of this control in the <see cref="Panel" />.
        /// </summary>
        /// <value>The zero based index.</value>
        public int Index { get; internal set; }

        /// <summary>
        ///     Determines whether this control can be focused.
        /// </summary>
        /// <value> <c>true</c> if the control can be focused; <c>false</c> otherwise.</value>
        public bool IsFocusable
        {
            get { return isFocusable; }
            internal set { isFocusable = value; }
        }

        public bool IsInited { get; protected set; }

        public Thickness Margin { get; set; }

        public VerticalAlignment VerticalAlignment
        {
            get { return verticalAlignment; }
            set
            {
                if (verticalAlignment == value)
                    return;
                verticalAlignment = value;
                if (!DesignMode)
                    InvalidateArrange();
            }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return horizontalAlignment; }
            set
            {
                if (horizontalAlignment == value)
                    return;
                horizontalAlignment = value;
                if (!DesignMode)
                    InvalidateArrange();
            }
        }

        /// <summary>
        ///     Gets the height and width of the control.
        /// </summary>
        /// <value>The <see cref="Vector2">Size</see> that represents the height, width and depth of the control in pixels.</value>
        public Vector3 Size
        {
            get { return new Vector3(Width, Height, Depth); }
        }

        public Vector3 RenderSize
        {
            get { return renderSize; }
            private set
            {
                var oldSize = renderSize;
                if (renderSize == value)
                    return;

                renderSize = value;
                if (!DesignMode)
                    OnSizeChanged(new SizeChangedEventArgs(oldSize, RenderSize));

                UpdateLayoutInternal();
                OnLayoutUpdated(EventArgs.Empty);
            }
        }

        public Vector3 DesiredSize { get; private set; }
        public Vector3 DesiredSizeWithMargins { get; private set; }

        public Matrix3x2 Transform
        {
            get { return transform; }
        }

        public float Width
        {
            get { return width; }
            set
            {
                if (width == value)
                    return;

                width = value;
                if (!DesignMode)
                    InvalidateMeasure();
            }
        }

        public bool IsArrangeValid { get; internal set; }
        public bool IsMeasureValid { get; internal set; }

        public float MinimumWidth
        {
            get { return minimumWidth; }
            set { minimumWidth = value; }
        }

        public float MinimumHeight
        {
            get { return minimumHeight; }
            set { minimumHeight = value; }
        }

        public float MinimumDepth
        {
            get { return minimumDepth; }
            set { minimumDepth = value; }
        }

        public float MaximumWidth
        {
            get { return maximumWidth; }
            set { maximumWidth = value; }
        }

        public float MaximumHeight
        {
            get { return maximumHeight; }
            set { maximumHeight = value; }
        }

        public float MaximumDepth
        {
            get { return maximumDepth; }
            set { maximumDepth = value; }
        }

        internal IEnumerable<BindingExpression> Bindings
        {
            get { return bindings.Values; }
        }

        public BehaviorCollection Behaviors
        {
            get { return behaviors; }
        }

        internal bool IsBeingRemoved { get; set; }

        /// <summary>
        ///     Currently not used.
        /// </summary>
        /// <summary>
        ///     Gets or sets a value indicating whether the mouse cursor is currently inside.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the mouse pointer is currently inside; otherwise,
        ///     <c>false</c>.
        /// </value>
        protected internal bool IsInside
        {
            get { return isInside && canRaiseEvents; }
            set { isInside = value; }
        }

        public float Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this control is clicked.
        /// </summary>
        /// <value><c>true</c> if this control is clicked; otherwise, <c>false</c>.</value>
        /// <remarks>
        ///     This value stays <c>true</c> for as long as the user presses the mouse button.
        /// </remarks>
        protected internal bool IsPressed { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this control is selected.
        /// </summary>
        /// <value><c>true</c> if this control is selected; otherwise, <c>false</c>.</value>
        protected internal bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnSelectedChanged(EventArgs.Empty);
            }
        }

        protected Overlay Overlay
        {
            get { return (this is Overlay) ? (Overlay)this : FindAncestor<Overlay>(); }
        }

        protected Direct2DDevice Device
        {
            get { return Overlay.Device; }
        }

        /// <summary>
        ///     Gets the absolute position in screen coordinates of the upper-left corner of this
        ///     control.
        /// </summary>
        /// <value>
        ///     A <see cref="Vector2" /> that represents the absolute
        ///     position of the upper-left corner in screen coordinates for this control.
        /// </value>
        public Vector3 AbsolutePosition
        {
            get { return absolutePosition; }
            protected set
            {
                var oldPosition = absolutePosition;
                if (absolutePosition == value)
                    return; 

                absolutePosition = value;
                
                UpdateLayoutInternal();
                if (!DesignMode)
                    OnPositionChanged(new PositionChangedEventArgs(oldPosition, absolutePosition));
            }
        }

        /// <summary>
        ///     Gets or sets a value that will be used by the interface to know whether that control can
        ///     raise events or not.
        /// </summary>
        /// <value><c>true</c> if the control can react to events; <c>false</c> otherwise.</value>
        /// <remarks>
        ///     Setting this property to <b>false</b> will lock the control in its current state.
        /// </remarks>
        public bool CanRaiseEvents
        {
            get { return canRaiseEvents; }
            set { canRaiseEvents = value; }
        }

        /// <summary>
        ///     Gets or sets a value that indicates whether the control can be interacted with.
        /// </summary>
        /// <remarks>
        ///     This consequently causes the <see cref="UIElement.CanRaiseEvents" /> property to be
        ///     set.
        /// </remarks>
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = canRaiseEvents = value; }
        }

        /// <summary>
        ///     Determines whether this control is focused.
        /// </summary>
        /// <value><c>true</c> if this control is focused; <c>false</c> otherwise.</value>
        public bool IsFocused { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this control is highlighted.
        /// </summary>
        /// <value><c>true</c> if this control is highlighted; otherwise, <c>false</c>.</value>
        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set
            {
                if (isHighlighted != value)
                {
                    isHighlighted = value;
                    OnHighlightedChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value that indicates whether the control is visible or not.
        /// </summary>
        /// <value><c>true</c> if the control is visible; <c>false</c> otherwise.</value>
        /// <remarks>
        ///     Setting this property to a different value that the one it had before the assignment,
        ///     will cause the UI to be recomputed if the control is not in <see cref="DesignMode" />
        /// </remarks>
        [Animatable]
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (value != isVisible)
                {
                    isVisible = value;
                    OnVisibleChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or Sets the parent control. When a new parent control is set the absolute position
        ///     of the child control is also computed.
        /// </summary>
        /// <value>The parent control.</value>
        public virtual UIElement Parent
        {
            get { return parent; }
            internal set
            {
                if (parent == value) return;
                parent = value;
                DesignMode = parent.DesignMode;
                OnParentChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Gets or sets the coordinates of the upper-left corner of the control relative to the
        ///     upper-left corner of its container.
        /// </summary>
        /// <value>
        ///     A Vector2 that represents the upper-left corner of the control relative to the
        ///     upper-left corner of its container.
        /// </value>
        /// <remarks>
        ///     If the controls's <see cref="Parent" /> is the <see cref="Odyssey.UserInterface.Controls.Overlay" />, the
        ///     <b>PositionV3</b> property value represents the upper-left corner of the control in
        ///     screen coordinates.
        /// </remarks>
        public Vector3 Position
        {
            get { return position; }
            set
            {
                if (position == value) return;

                position = value;

                if (DesignMode) return;
                InvalidateArrange();
                OnPositionChanged(new PositionChangedEventArgs(position, position));
            }
        }

        internal Vector3 PositionOffsets
        {
            get { return positionOffsets; }
            set { positionOffsets = value; }
        }

        public AnimationController Animator
        {
            get { return animator; }
        }
    }
}