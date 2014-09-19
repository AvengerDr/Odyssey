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

using System;
using MiniUI;
using Odyssey.Content;
using Odyssey.Engine;
using Odyssey.UserInterface;
using Odyssey.UserInterface.Controls;
using Odyssey.UserInterface.Style;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System.Drawing;
using Color = SharpDX.Color;

#endregion Using Directives

namespace DataBinding
{
    public class DataBindingApplication : Component, IWindowService
    {
        private readonly SimpleDeviceManager deviceManager;
        private readonly Direct2DDevice direct2DDevice;
        private readonly RenderForm form;
        private readonly Overlay overlay;
        private readonly UserInterfaceManager uiManager;

        public DataBindingApplication()
        {
            var services = new ServiceRegistry();
            // Needs to be declared before the D3D device manager as it hooks to
            // the D3D device creation event
            var d2dDeviceManager = new Direct2DDeviceManager(services);
            deviceManager = ToDispose(new SimpleDeviceManager(services)
            {
                HorizontalDpi = 96.0f,
                VerticalDpi = 96.0f,
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                PreferredBackBufferFormat = Format.R8G8B8A8_UNorm,
                IsFullScreen = false,
                IsStereo = false,
                PreferredGraphicsProfile = new[] {FeatureLevel.Level_11_0,},
                PreferredMultiSampleCount = 1
            });
            form = new RenderForm("OdysseyUI Test")
            {
                ClientSize = new Size(deviceManager.PreferredBackBufferWidth, deviceManager.PreferredBackBufferHeight),
            };

            deviceManager.CreateDevice(form.Handle, DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport);

            var content = new ContentManager(services);
            var styleManager = new StyleManager(services);
            uiManager = new DesktopUserInterfaceManager(services);

            services.AddService(typeof (IStyleService), styleManager);
            services.AddService(typeof(IDirect2DService), d2dDeviceManager);
            services.AddService(typeof (IUserInterfaceState), uiManager);
            services.AddService(typeof (IWindowService), this);

            content.LoadAssetList("Assets/Assets.yaml");

            overlay = ToDispose(SampleOverlay.New(services));
            overlay.Initialize();
            uiManager.Initialize();
            uiManager.CurrentOverlay = overlay;
        }

        public object NativeWindow
        {
            get { return form; }
        }

        public void Run()
        {
            deviceManager.Context.OutputMerger.SetTargets(deviceManager.BackBuffer);
            RenderLoop.Run(form, () =>
            {
                deviceManager.Context.ClearRenderTargetView(deviceManager.BackBuffer, Color.Black);
                Render();
                deviceManager.SwapChain.Present(0, PresentFlags.None);
            });
        }

        private void Render()
        {
            uiManager.Update();
            overlay.Display();
        }
    }
}