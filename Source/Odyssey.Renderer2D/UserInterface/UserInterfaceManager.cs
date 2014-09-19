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

using Odyssey.Engine;
using Odyssey.Interaction;
using Odyssey.UserInterface.Controls;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

#endregion Using Directives

namespace Odyssey.UserInterface
{
    /// <summary>
    /// Odyssey User Interface manager.
    /// </summary>
    public abstract class UserInterfaceManager : IUserInterfaceState
    {
        private const int maxEvents = 32;
        private readonly PointerManager pointerManager;
        private readonly PointerState pointerState;
        private readonly Queue<PointerEventArgs> recentEvents;
        private readonly IServiceRegistry services;
        private int eventIndex;
        private PointerPlatform pointerPlatform;

        protected UserInterfaceManager(IServiceRegistry services)
        {
            Contract.Requires<ArgumentNullException>(services != null, "services");
            this.services = services;
            pointerManager = new PointerManager(services);
            recentEvents = new Queue<PointerEventArgs>();
            pointerState = new PointerState();
        }

        /// <summary>
        /// Gets or sets a reference to current <see cref="Odyssey.UserInterface.Controls.Overlay"/> displayed on the screen.
        /// </summary>
        /// <value>The current <see cref="Odyssey.UserInterface.Controls.Overlay"/> object.</value>
        public IOverlay CurrentOverlay { get; set; }

        internal static PointerEventArgs LastPointerEvent { get; private set; }

        internal PointerPlatform PointerPlatform
        {
            get { return pointerPlatform; }
            set { pointerPlatform = value; }
        }

        protected IServiceRegistry Services
        {
            get { return services; }
        }

        public PointerManager PointerManager
        {
            get { return pointerManager; }
        }

        public virtual void Initialize()
        {
            IWindowService windowService = services.GetService<IWindowService>();
            pointerPlatform.Initialize(windowService.NativeWindow);
        }

        public void SetOverlay(IOverlay overlay)
        {
            CurrentOverlay = overlay;
        }

        public virtual void Update()
        {
            pointerManager.Update();
            pointerManager.GetState(pointerState);

            foreach (var point in pointerState.Points)
                AddEvent(point);

            var unhandledEvents = recentEvents.Where(p => !p.Handled);
            foreach (var pointerEvent in unhandledEvents)
                Process(pointerEvent);
        }

        protected void Process(PointerEventArgs pointerEvent)
        {
            PointerPoint point = pointerEvent.CurrentPoint;

            switch (point.EventType)
            {
                case PointerEventType.CaptureLost:
                    break;

                case PointerEventType.Entered:
                    break;

                case PointerEventType.Exited:
                    break;

                case PointerEventType.Moved:
                    CurrentOverlay.ProcessPointerMovement(pointerEvent);
                    break;

                case PointerEventType.Pressed:
                    CurrentOverlay.ProcessPointerPress(pointerEvent);
                    break;

                case PointerEventType.Released:
                    CurrentOverlay.ProcessPointerRelease(pointerEvent);
                    break;

                case PointerEventType.WheelChanged:
                    break;

                default:
                    throw new ArgumentOutOfRangeException("point");
            }
        }

        

        private void AddEvent(PointerPoint point)
        {
            if (recentEvents.Count == maxEvents)
                recentEvents.Dequeue();

            recentEvents.Enqueue(new PointerEventArgs(eventIndex++, point));
        }

        #region UI Input

        //public static void ProcessKeyDown(KeyEventArgs e)
        //{
        //    if (e.KeyCode == Key.None)
        //        return;

        //    if (CurrentOverlay.FocusedControl != null)
        //        CurrentOverlay.FocusedControl.ProcessKeyDown(e);
        //}

        //public static void ProcessKeyUp(KeyEventArgs e)
        //{
        //    if (e.KeyCode == Key.None)
        //        return;

        //    if (CurrentOverlay.FocusedControl != null)
        //        CurrentOverlay.FocusedControl.ProcessKeyUp(e);
        //}

        //static void ProcessKeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (CurrentOverlay.FocusedControl != null)
        //        CurrentOverlay.FocusedControl.ProcessKeyPress(e);
        //}

        #endregion UI Input
    }
}