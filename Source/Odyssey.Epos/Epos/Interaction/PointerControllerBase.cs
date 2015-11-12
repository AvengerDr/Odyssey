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

using System.Collections.Generic;
using Odyssey.Core;
using Odyssey.Engine;
using Odyssey.Interaction;
using Odyssey.Epos;
using SharpDX;

#endregion

namespace Odyssey.Epos.Interaction
{
    public abstract class PointerControllerBase : ControllerBase
    {
        private IKeyboardService keyboardService;
        private IPointerService pointerService;
        private List<Keys> lPrevKeyPressed; 
        protected Vector2 ScreenSize { get; private set; }
        protected IKeyboardService KeyboardService { get { return keyboardService; } }

        protected bool IsLeftButtonDragging { get; set; }
        protected bool IsRightButtonDragging { get; set; }
        
        protected Vector2 CurrentPointerPosition { get; set; }
        protected Vector2 PreviousPointerPosition { get; set; }

        protected PointerControllerBase(IServiceRegistry services) : base(services)
        {
            lPrevKeyPressed = new List<Keys>();
        }

        public override void BindToEntity(Entity source)
        {
            base.BindToEntity(source);
            pointerService = Services.GetService<IPointerService>();
            var deviceSettings = Services.GetService<IDirectXDeviceSettings>();
            ScreenSize = new Vector2(deviceSettings.PreferredBackBufferWidth, deviceSettings.PreferredBackBufferHeight);
            keyboardService = Services.GetService<IKeyboardService>();
        }

        public override void Update(ITimeService time)
        {
            var pointerState = pointerService.GetState();
            foreach (var point in pointerState.Points)
            {
                switch (point.EventType)
                {
                    case PointerEventType.Pressed:
                        PointerPressed(point, time);
                        break;

                    case PointerEventType.Moved:
                        PointerMoved(point, time);
                        break;

                    case PointerEventType.Released:
                        PointerReleased(point, time);
                        break;
                }
            }

            var keyboardState = keyboardService.GetState();

            var lKeys = new List<Keys>();
            keyboardState.GetDownKeys(lKeys);
            foreach (var keyDown in lKeys)
            {
                KeyPressed(keyDown, time);
            }

            foreach (var keyReleased in lPrevKeyPressed)
            {
                if (keyboardState.IsKeyReleased(keyReleased))
                    KeyReleased(keyReleased, time);
            }
            lPrevKeyPressed = lKeys;
        }

        protected abstract void PointerPressed(PointerPoint point, ITimeService time);
        protected abstract void PointerMoved(PointerPoint point, ITimeService time);
        protected abstract void PointerReleased(PointerPoint point, ITimeService time);

        protected virtual void KeyPressed(Keys key, ITimeService time) { }
        protected virtual void KeyReleased(Keys key, ITimeService time) { }

        protected static bool CheckButton(PointerUpdateKind update, MouseButtons button, bool pressed)
        {
            switch (update)
            {
                case PointerUpdateKind.LeftButtonPressed:
                    return (button == MouseButtons.Left) && pressed;

                case PointerUpdateKind.LeftButtonReleased:
                    return (button == MouseButtons.Left) && !pressed;

                case PointerUpdateKind.MiddleButtonPressed:
                    return (button == MouseButtons.Middle) && pressed;

                case PointerUpdateKind.MiddleButtonReleased:
                    return (button == MouseButtons.Middle) && !pressed;

                case PointerUpdateKind.RightButtonPressed:
                    return (button == MouseButtons.Right) && pressed;
                case PointerUpdateKind.RightButtonReleased:
                    return (button == MouseButtons.Right) && !pressed;

                default:
                    return false;
            }
        }

        public static MouseButtons Translate(PointerUpdateKind update)
        {
            switch (update)
            {
                case PointerUpdateKind.LeftButtonPressed:
                case PointerUpdateKind.LeftButtonReleased:
                    return MouseButtons.Left;

                case PointerUpdateKind.MiddleButtonPressed:
                case PointerUpdateKind.MiddleButtonReleased:
                    return MouseButtons.Middle;

                case PointerUpdateKind.RightButtonPressed:
                case PointerUpdateKind.RightButtonReleased:
                    return MouseButtons.Right;

                default:
                    return MouseButtons.None;
            }

        }
    }
}