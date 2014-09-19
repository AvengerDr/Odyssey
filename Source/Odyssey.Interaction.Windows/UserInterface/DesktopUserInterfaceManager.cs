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

#endregion Using Directives

namespace Odyssey.UserInterface
{
    public class DesktopUserInterfaceManager : UserInterfaceManager
    {
        private readonly KeyboardManager keyboardManager;
        private KeyboardState keyboardState;

        public DesktopUserInterfaceManager(IServiceRegistry services)
            : base(services)
        {
            keyboardManager = new KeyboardManager(services);
            PointerPlatform = new DesktopPointerPlatform(PointerManager);
        }

        public override void Initialize()
        {
            base.Initialize();
            IWindowService windowService = Services.GetService<IWindowService>();
            KeyboardPlatform keyboardPlatform = new DesktopKeyboardPlatform(keyboardManager);
            keyboardPlatform.Initialize(windowService.NativeWindow);
            keyboardPlatform.KeyUp += (s, e) => ((IDesktopOverlay) CurrentOverlay).ProcessKeyUp(e);
            keyboardPlatform.KeyDown += (s, e) => ((IDesktopOverlay)CurrentOverlay).ProcessKeyDown(e);
            keyboardManager.Associate(keyboardPlatform);
        }

        public override void Update()
        {
            // read the current keyboard state
            keyboardManager.Update();
            keyboardState = keyboardManager.GetState();

            base.Update(); 
        }
    }
}