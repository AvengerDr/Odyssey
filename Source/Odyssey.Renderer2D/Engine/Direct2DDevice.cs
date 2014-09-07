#region Using Directives

using Odyssey.Content;
using Odyssey.Graphics;
using Odyssey.Graphics.Drawing;
using Odyssey.UserInterface.Style;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using System;
using System.Diagnostics.Contracts;
using Brush = Odyssey.Graphics.Brush;
using D2DFactory = SharpDX.Direct2D1.Factory1;
using Device = SharpDX.Direct2D1.Device;
using DeviceContext = SharpDX.Direct2D1.DeviceContext;
using DWFactory = SharpDX.DirectWrite.Factory1;
using Ellipse = Odyssey.Graphics.Drawing.Ellipse;
using FactoryType = SharpDX.Direct2D1.FactoryType;

#endregion Using Directives

namespace Odyssey.Engine
{
    public class Direct2DDevice : Component, IDirect2DDevice
    {
        private readonly DebugLevel debugLevel;
        private readonly IDirectXDeviceService dx11Service;
        private readonly IServiceRegistry services;
        private BackBufferSurface backBuffer;
        private Device device;
        private DeviceContext deviceContext;
        private DWFactory directWriteFactory;
        private D2DFactory factory;
        private Direct2DSurface target;

        public Direct2DDevice(IServiceRegistry services)
            : this(services, DebugLevel.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Direct2DDevice" />.
        /// events via
        /// <see cref="IDirectXDeviceService" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="services" /> is null.</exception>
        public Direct2DDevice(IServiceRegistry services, DebugLevel debugLevel)
        {
            Contract.Requires<ArgumentNullException>(services != null, "services");
            dx11Service = services.GetService<IDirectXDeviceService>();
            this.services = services;
            this.debugLevel = debugLevel;
        }

        public Direct2DDevice(IServiceRegistry services, SharpDX.Direct3D11.Device d3dDevice, DebugLevel debugLevel)
        {
            this.services = services;
            using (var dxgiDevice = d3dDevice.QueryInterface<SharpDX.DXGI.Device>())
            {
                factory = ToDispose(new SharpDX.Direct2D1.Factory1(FactoryType.SingleThreaded, debugLevel));
                device = ToDispose(new Device(factory, dxgiDevice));
                deviceContext = ToDispose(new DeviceContext(device, DeviceContextOptions.None));
            }

            backBuffer = ToDispose(BackBufferSurface.New(this));
            backBuffer.Initialize();
            
            directWriteFactory = ToDispose(new DWFactory());
        }

        public Direct2DSurface BackBuffer
        {
            get { return backBuffer; }
        }

        public float HorizontalDpi
        {
            get { return deviceContext.DotsPerInch.Width; }
        }

        public float VerticalDpi
        {
            get { return deviceContext.DotsPerInch.Height; }
        }

        /// <summary>
        /// Gets a reference to the Direct2D device.
        /// </summary>
        public Device Device
        {
            get { return device; }
        }

        /// <summary>
        /// Gets a reference to the default <see cref="Direct2D1.DeviceContext" />.
        /// </summary>
        public DeviceContext DeviceContext
        {
            get { return deviceContext; }
        }

        /// <summary>
        /// Gets a reference to the default <see cref="SharpDX.DirectWrite.Factory1" />.
        /// </summary>
        public DWFactory DirectWriteFactory
        {
            get { return directWriteFactory; }
        }

        public bool IsDebugMode
        {
            get { return debugLevel != DebugLevel.None; }
        }

        public Direct2DSurface Target
        {
            get { return target; }
            set
            {
                if (target != value)
                {
                    target = value;
                    DeviceContext.Target = target;
                }
            }
        }

        internal IServiceRegistry Services
        {
            get { return services; }
        }

        internal void CreateResources()
        { }

        public void DrawGeometry(Graphics.Drawing.Geometry geometry, Brush brush)
        {
            deviceContext.DrawGeometry(geometry, brush);
        }

        public void DrawRectangle(Shape shape, Brush brush, float strokeThickness = 1.0f)
        {
            deviceContext.DrawRectangle(shape.BoundingRectangle, brush, strokeThickness);
        }

        public void DrawEllipse(Ellipse ellipse, Brush brush, float strokeThickness = 1.0f)
        {
            deviceContext.DrawEllipse(ellipse, brush, strokeThickness);
        }

        public void FillEllipse(Ellipse ellipse, Brush brush)
        {
            deviceContext.FillEllipse(ellipse, brush);
        }

        public void DrawLine(Line line, Brush brush, float strokeThickness = 1.0f)
        {
            deviceContext.DrawLine(line.P0, line.P1, brush, strokeThickness);
        }

        public void DrawText(string text, TextFormat textFormat, RectangleF layoutRect, Brush foregroundBrush,
            DrawTextOptions textOptions = DrawTextOptions.None)
        {
            deviceContext.DrawText(text, textFormat, layoutRect, foregroundBrush, textOptions);
        }

        public void FillGeometry(Graphics.Drawing.Geometry geometry, Brush brush)
        {
            deviceContext.FillGeometry(geometry, brush);
        }

        public void FillRectangle(Shape shape, Brush brush)
        {
           FillRectangle(shape.BoundingRectangle, brush);
        }

        public void FillRectangle(RectangleF rectangle, Brush brush)
        {
            deviceContext.FillRectangle(rectangle, brush);
        }

        public void SetTextAntialias(AntialiasMode antialiasMode)
        {
            deviceContext.AntialiasMode = antialiasMode;
        }

        public Matrix3x2 Transform
        {
            get { return deviceContext.Transform; }
            set { deviceContext.Transform = value; }
        }

        public void SetPrimitiveBlend(PrimitiveBlend blendMode)
        {
            deviceContext.PrimitiveBlend = blendMode; 
        }

        /// <summary>
        /// Diposes all resources associated with the current <see cref="Direct2DDevice" /> instance.
        /// </summary>
        /// <param name="disposeManagedResources">Indicates whether to dispose management resources.</param>
        protected override void Dispose(bool disposeManagedResources)
        {
            base.Dispose(disposeManagedResources);

            DisposeAll();
        }

        /// <summary>
        /// Disposes the <see cref="Direct2DDevice.Device" />, <see cref="DeviceContext" /> and its render target
        /// associated with the current <see cref="Direct2DDevice" /> instance.
        /// </summary>
        public void DisposeAll()
        {
            device.ClearResources(0);

            if (target != backBuffer)
                RemoveAndDispose(ref target);
            RemoveAndDispose(ref backBuffer);
            
            RemoveAndDispose(ref directWriteFactory);
            RemoveAndDispose(ref factory);
            RemoveAndDispose(ref deviceContext);
            RemoveAndDispose(ref device);
        }

        #region Operators

        public static implicit operator D2DFactory(Direct2DDevice from)
        {
            return from == null ? null : from.factory;
        }

        public static implicit operator DeviceContext(Direct2DDevice from)
        {
            return from == null ? null : from.deviceContext;
        }

        public static implicit operator SharpDX.Direct3D11.Device(Direct2DDevice from)
        {
            return from == null ? null : from.dx11Service.DirectXDevice.Device;
        }

        public static implicit operator SharpDX.DirectWrite.Factory(Direct2DDevice from)
        {
            return from == null ? null : from.directWriteFactory;
        }

        #endregion Operators
    }
}