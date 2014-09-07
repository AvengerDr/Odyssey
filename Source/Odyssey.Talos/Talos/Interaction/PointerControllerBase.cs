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

#endregion

#region Using Directives

using Odyssey.Engine;
using Odyssey.Interaction;
using Odyssey.Talos;
using SharpDX;

#endregion

namespace Odyssey.Talos.Interaction
{
    public abstract class PointerControllerBase : ControllerBase
    {
        protected IKeyboardService keyboardService;
        private IPointerService pointerService;
        protected Vector2 ScreenSize { get; private set; }

        protected PointerControllerBase(IServiceRegistry services) : base(services) {}

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
            PointerState state = pointerService.GetState();
            foreach (var point in state.Points)
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
        }

        protected abstract void PointerPressed(PointerPoint point, ITimeService time);
        protected abstract void PointerMoved(PointerPoint point, ITimeService time);
        protected abstract void PointerReleased(PointerPoint point, ITimeService time);
    }
}