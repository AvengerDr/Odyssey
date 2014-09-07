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

using System.Collections.Generic;
using Odyssey.Content;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.Interaction;
using Odyssey.UserInterface.Style;
using SharpDX;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using SharpDX.Direct2D1;

#endregion Using Directives

namespace Odyssey.UserInterface.Controls
{
    public sealed class Overlay : ContainerControl, IOverlay
    {
        private const string DefaultControlTheme = "DefaultTheme";
        private const string DefaultTextTheme = "DefaultText";
        private readonly Direct2DDevice device;
        private readonly IServiceRegistry services;
        private readonly IStyleService styleService;
        private readonly Theme theme;
        private UIElement captureElement;

        #region Properties

        public Theme Theme { get { return theme; } }

        public string TextTheme { get; private set; }

        public new Direct2DDevice Device { get { return device; } }

        internal UIElement CaptureElement
        {
            get { return captureElement; }
            set
            {
                if (value != null)
                    value.IsPointerCaptured = true;
                else if (captureElement != null)
                {
                    captureElement.IsPointerCaptured = false;
                    captureElement.OnPointerCaptureChanged(UserInterfaceManager.LastPointerEvent);
                }
                captureElement = value;
            }
        }

        internal UIElement EnteredElement { get; set; }
        internal UIElement FocusedElement { get; set; }

        public IServiceRegistry Services { get { return services; } }

        #endregion Properties

        public Overlay(IServiceRegistry services)
            : base("Empty")
        {
            Contract.Requires<ArgumentNullException>(services != null, "services");

            this.services = services;
            device = services.GetService<IDirect2DService>().Direct2DDevice;

            IsInside = true;
            IsFocusable = true;

            TextTheme = DefaultTextTheme;
            styleService = ToDispose(services.GetService<IStyleService>());
            theme = styleService.GetTheme(DefaultControlTheme);
            
            EnteredElement = this;
            FocusedElement = this;
            Overlay = this;
        }

        public void BeginDesign()
        {
            DesignMode = true;
        }

        public override bool Contains(Vector2 cursorLocation)
        {
            return true;
        }

        public void Display(bool clear = false)
        {
            Device.Target = Device.BackBuffer;

            DeviceContext context = Device;

            context.BeginDraw();
            if (clear)
                context.Clear(Color.Transparent);
            Render();
            context.EndDraw();
        }

        public void EndDesign()
        {
            Initialize();
            DesignMode = false;
        }

        public override void Render()
        {
            foreach (var control in Controls.Where(control => control.IsVisible))
                control.Render();
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            base.Dispose(disposeManagedResources);
            if (disposeManagedResources)
            {
                foreach (UIElement uiElement in Controls)
                    uiElement.Dispose();
            }
            IsInited = false;
        }

        protected override void OnInitialized(ControlEventArgs e)
        {
            base.OnInitialized(e);
            Layout();
            IsInited = true;
        }

        protected internal override void Measure()
        {
            var settings = Services.GetService<IDirectXDeviceSettings>();
            Width = settings.PreferredBackBufferWidth;
            Height = settings.PreferredBackBufferHeight;
            base.Measure();
        }

        void IOverlay.ProcessPointerMovement(PointerEventArgs e)
        {
            // If an element as captured the pointer, send the event to it first
            if (CaptureElement != null)
                e.Handled = CaptureElement.ProcessPointerMovement(e);

            if (e.Handled)
                return;

            //Proceeds with the rest
            foreach (UIElement control in TreeTraversal.PostOrderInteractionVisit(this))
            {
                e.Handled = control.ProcessPointerMovement(e);
                if (e.Handled)
                    break;
            }

            if (e.Handled) return;

            ProcessPointerMovement(e);
            e.Handled = true;
        }

        void IOverlay.ProcessPointerPress(PointerEventArgs e)
        {
            // If an element as captured the pointer, send the event to it first
            if (CaptureElement != null)
                e.Handled = CaptureElement.ProcessPointerPressed(e);

            if (e.Handled)
                return;
            //Proceeds with the rest
            foreach (UIElement control in TreeTraversal.PostOrderInteractionVisit(this))
            {
                e.Handled = control.ProcessPointerPressed(e);
                if (e.Handled)
                    break;
            }

            if (e.Handled)
                return;

            ProcessPointerPressed(e);
            e.Handled = true;
        }

        void IOverlay.ProcessPointerRelease(PointerEventArgs e)
        {
            // If an element as captured the pointer, send the event to it first
            if (CaptureElement != null)
                e.Handled = CaptureElement.ProcessPointerRelease(e);

            if (e.Handled)
                return;

            //Proceeds with the rest
            foreach (UIElement control in TreeTraversal.PostOrderInteractionVisit(this))
            {
                e.Handled = control.ProcessPointerRelease(e);
                if (e.Handled)
                    break;
            }

            if (e.Handled)
                return;

            ProcessPointerRelease(e);
            e.Handled = true;
        }

        public override void Update(ITimeService time)
        {
            foreach (var control in TreeTraversal.PreOrderVisit(this).Skip(1))
                control.Update(time);
        }

        #region IResourceProvider

        protected override bool ContainsResource(string resourceName)
        {
            return TreeTraversal.PreOrderVisit(this).Any(c=> string.Equals(c.Name, resourceName));
        }

        protected override TResource GetResource<TResource>(string resourceName)
        {
            return TreeTraversal.PreOrderVisit(this).First(c => string.Equals(c.Name, resourceName)) as TResource;
        }

        protected override IEnumerable<IResource> Resources
        {
            get { return TreeTraversal.PreOrderVisit(this); }
        }

        #endregion
    }
}