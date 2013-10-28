﻿#if !ODYSSEY_ENGINE && DIRECTX11_1 &&!WP8
using SharpDX;
using System;

namespace Odyssey.Engine
{
    public abstract class TargetBase : Component, IDirectXTarget, IDeviceDependentComponent
    {
        protected SharpDX.Direct3D11.RenderTargetView renderTargetView;
        protected SharpDX.Direct3D11.RenderTargetView renderTargetViewRight;
        protected SharpDX.Direct3D11.DepthStencilView depthStencilView;
        protected SharpDX.Direct3D11.Texture2D backBuffer;

        protected SharpDX.Direct2D1.Bitmap1 bitmapTarget;
        protected SharpDX.Direct2D1.Bitmap1 bitmapTargetRight;
        protected DeviceManager DeviceManager { get; private set; }

        /// <summary>
        /// Gets the Direct3D RenderTargetView used by this target.
        /// </summary>
        public SharpDX.Direct3D11.RenderTargetView RenderTargetView { get { return renderTargetView; } }
        public SharpDX.Direct3D11.RenderTargetView RenderTargetViewRight { get { return renderTargetViewRight; } }
        public SharpDX.Direct3D11.Texture2D BackBuffer { get { return backBuffer; } }

        /// <summary>
        /// Gets the Direct3D DepthStencilView used by this target.
        /// </summary>
        public SharpDX.Direct3D11.DepthStencilView DepthStencilView { get { return depthStencilView; } }

        /// <summary>
        /// Gets the Direct2D RenderTarget used by this target.
        /// </summary>
        public SharpDX.Direct2D1.Bitmap1 BitmapTarget { get { return bitmapTarget; } }
        public SharpDX.Direct2D1.Bitmap1 BitmapTargetRight { get { return bitmapTargetRight; } }
        /// <summary>
        /// Event fired when size of the underlying control is changed
        /// </summary>
        public event EventHandler<RenderEventArgs> SizeChanged;

        protected virtual void OnSizeChanged(RenderEventArgs e)
        {
            if (SizeChanged != null)
                SizeChanged(this, e);
        }

        /// <summary>
        /// Event fired when rendering is performed by this target
        /// </summary>
        public event EventHandler<RenderEventArgs> Render;

        /// <summary>
        /// Render all events registered on event <see cref="Render"/>
        /// </summary>
        public virtual void RenderAll()
        {
            if (Render != null)
                Render(this, new RenderEventArgs(this));
        }


        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="deviceManager">The device manager</param>
        public virtual void Initialize(InitializeDirectXEventArgs args)
        {
            this.DeviceManager = (DeviceManager)args.DirectX;

            // If the DPI is changed, we need to perform a OnSizeChanged event
            DeviceManager.OnDpiChanged -= devices_OnDpiChanged;
            DeviceManager.OnDpiChanged += devices_OnDpiChanged;
            DeviceManager.Dpi = args.Settings.Dpi;

        }


        public abstract void Close();

        private void devices_OnDpiChanged(DeviceManager obj)
        {
            if (SizeChanged != null)
                SizeChanged(this, new RenderEventArgs(this));
        }

        IDirect2DProvider IDirectXTarget.Direct2D
        { get { return DeviceManager; } }

        IDirectWriteProvider IDirectXTarget.DirectWrite
        {
            get { return DeviceManager; }
        }

        IDirect3DProvider IDirectXTarget.Direct3D
        { get { return DeviceManager; } }

    }
}
#endif
