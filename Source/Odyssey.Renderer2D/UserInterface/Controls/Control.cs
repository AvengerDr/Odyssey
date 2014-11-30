﻿#region License

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

#endregion

#region Using Directives

using System;
using System.Collections.Generic;
using Odyssey.Content;
using Odyssey.Graphics.Drawing;
using Odyssey.UserInterface.Style;
using Odyssey.Logging;
using SharpDX.Mathematics;

#endregion

namespace Odyssey.UserInterface.Controls
{
    public abstract class Control : UIElement, IResourceProvider
    {
        private string controlStyleClass;
        private ControlStyle style;
        private TextStyle textStyle;
        private string textStyleClass;
        private VisualState visualState;

        protected Control(string controlStyleClass, string textStyleClass = TextStyle.Default)
        {
            this.controlStyleClass = controlStyleClass;
            this.textStyleClass = textStyleClass;
        }

        public Thickness Padding { get; set; }

        public virtual float ClientAreaHeight { get { return Height - Padding.Vertical; }}
        public virtual float ClientAreaWidth { get { return Width - Padding.Horizontal; } }

        public string StyleClass
        {
            get { return controlStyleClass; }
            set
            {
                if (string.Equals(controlStyleClass, value))
                    return;
                controlStyleClass = value;
                if (!DesignMode)
                    ApplyControlDescription();
            }
        }

        protected bool IsVisual
        {
            get { return VisualState != null; }
        }

        /// <summary>
        /// Gets or sets the <see cref = "TextStyle" /> to use for this control.
        /// </summary>
        /// <value>The <see cref = "TextStyle" /> object that contains information on how to
        /// format the text of this control.</value>
        public TextStyle TextStyle
        {
            get { return textStyle; }
            private set
            {
                if (textStyle == value) return;
                textStyle = value;
                OnTextStyleChanged(EventArgs.Empty);
            }
        }

        public string TextStyleClass
        {
            get { return textStyleClass; }
            set
            {
                if (string.Equals(textStyleClass, value))
                    return;

                textStyleClass = value;
                
                if (DesignMode)
                    return;

                ApplyTextDescription();
                InvalidateMeasure();
                OnTextStyleChanged(EventArgs.Empty);
            }
        }

        protected ControlStatus ActiveStatus { get; set; }

        protected VisualState VisualState
        {
            get { return visualState; }
        }

        /// <summary>
        /// Gets or sets the <see cref = "Style" /> to use for this control.
        /// </summary>
        /// <value>The <see cref = "Style" /> object that contains information on how to style
        /// the text appearance of this control.</value>
        public ControlStyle Style
        {
            get { return style; }
            private set
            {
                if (style == value) return;

                style = value;
                OnControlDefinitionChanged(EventArgs.Empty);
            }
        }

        public override void Render()
        {
            if (!IsVisual)
                return;

            foreach (var child in Controls)
                child.Render();
        }

        /// <summary>
        /// Occurs when the <see cref="UserInterface.Style.ControlStyle"/> property value changes.
        /// </summary>
        public event EventHandler<EventArgs> ControlStyleChanged;

        /// <summary>
        /// Occurs when the <see cref="UserInterface.Style.TextStyle"/> property value changes.
        /// </summary>
        public event EventHandler<EventArgs> TextStyleChanged;

        protected internal override UIElement Copy()
        {
            var newControl = (Control) base.Copy();
            newControl.Style = Style;
            newControl.Padding = Padding;
            newControl.TextStyleClass = TextStyleClass;
            CopyEvents(typeof (Control), this, newControl);
            return newControl;
        }

        protected override Vector3 ArrangeOverride(Vector3 availableSizeWithoutMargins)
        {
            foreach (var child in Controls)
                child.Arrange(availableSizeWithoutMargins - MarginInternal);
            return base.ArrangeOverride(availableSizeWithoutMargins);
        }

        protected override Vector3 MeasureOverride(Vector3 availableSizeWithoutMargins)
        {
            TopLeftPosition = new Vector3(Padding.Left, Padding.Top, 0);

            foreach (var child in Controls)
                child.Measure(availableSizeWithoutMargins - MarginInternal);

            if (float.IsNaN(Width) || float.IsNaN(Height) || float.IsNaN(Depth))
            {
                return availableSizeWithoutMargins - MarginInternal;
            }
            else return Size - MarginInternal;
        }

        private void ApplyControlDescription()
        {
            if (StyleClass == ControlStyle.Empty)
                return;

            if (!Overlay.Theme.ContainsResource(StyleClass))
            {
                LogEvent.UserInterface.Warning("Style '{0}' not found", StyleClass);
                return;
            }
            var controlStyle = Overlay.Theme.GetResource<ControlStyle>(StyleClass);

            if (Width == 0 && controlStyle.Width > 0)
            {
                Width = controlStyle.Width;
            }

            if (Height == 0 && controlStyle.Height > 0)
            {
                Height = controlStyle.Height;
            }

            if (Padding.IsEmpty && !controlStyle.Padding.IsEmpty)
                Padding = controlStyle.Padding;

            TopLeftPosition = new Vector3(Padding.Left, Padding.Top, 0);
            RemoveAndDispose(ref visualState);
            visualState = ToDispose(controlStyle.CreateVisualState(this));
            visualState.Initialize();

            foreach (var s in visualState)
                Controls.Add(s);
            Style = controlStyle;
        }

        protected void ApplyTextDescription()
        {
            if (!Overlay.Theme.ContainsResource(TextStyleClass))
            {
                LogEvent.UserInterface.Warning("TextStyle '{0}' not found", TextStyleClass);
                return;
            }

            TextStyle = Overlay.Theme.GetResource<TextStyle>(TextStyleClass);
        }

        /// <summary>
        /// Raises the <see cref="ControlStyleChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnControlDefinitionChanged(EventArgs e)
        {
            if (!DesignMode)
                RaiseEvent(ControlStyleChanged, this, e);
        }

        protected override void OnDesignModeChanged(EventArgs e)
        {
            base.OnDesignModeChanged(e);
            if (!IsVisual || ActiveStatus == ControlStatus.None)
                return;

            foreach (Shape element in VisualState)
                element.DesignMode = DesignMode;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            foreach (UIElement element in Controls)
            {
                element.Initialize();
            }
            if (string.Equals(StyleClass, "Empty")) return;
            ActiveStatus = IsEnabled ? ControlStatus.Enabled : ControlStatus.Disabled;
        }

        protected override void OnInitializing(EventArgs e)
        {
            base.OnInitializing(e);
            ApplyControlDescription();
            ApplyTextDescription();
        }

        /// <summary>
        /// Raises the <see cref="TextStyleChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event
        /// data.</param>
        protected virtual void OnTextStyleChanged(EventArgs e)
        {
            if (!DesignMode)
                RaiseEvent(TextStyleChanged, this, e);
        }

        public Shape GetShape(string shapeName)
        {
            return VisualState[shapeName];
        }

        #region IResourceProvider

        protected virtual IEnumerable<IResource> Resources
        {
            get { return VisualState; }
        }

        TResource IResourceProvider.GetResource<TResource>(string resourceName)
        {
            return GetResource<TResource>(resourceName);
        }

        IEnumerable<IResource> IResourceProvider.Resources
        {
            get { return Resources; }
        }

        bool IResourceProvider.ContainsResource(string resourceName)
        {
            return ContainsResource(resourceName);
        }

        protected virtual bool ContainsResource(string resourceName)
        {
            return VisualState.ContainsResource(resourceName);
        }

        protected virtual TResource GetResource<TResource>(string resourceName)
            where TResource : class, IResource
        {
            return GetShape(resourceName) as TResource;
        }

        #endregion
    }
}