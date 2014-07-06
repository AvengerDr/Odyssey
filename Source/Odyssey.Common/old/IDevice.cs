﻿using SharpDX.Direct2D1;
using Device1 = SharpDX.Direct3D11.Device1;
using DeviceContext1 = SharpDX.Direct3D11.DeviceContext1;
using Factory = SharpDX.DirectWrite.Factory;

namespace Odyssey.Engine
{
#if !WP8
    public interface IDirect2DProvider
    {
        Device Device { get;  }
        DeviceContext Context { get; }
        Factory1 Factory { get; }
    }

    public interface IDirectWriteProvider
    {
        Factory Factory { get; }
    }
#endif

    public interface IDirect3DProvider
    {
#if DIRECTX11_1
        Device1 Device { get;  }
        DeviceContext1 Context { get;  }
#else
        SharpDX.Direct3D11.Device Device { get; }
        SharpDX.Direct3D11.DeviceContext Context { get; }
#endif

    }

    public interface IDirectXProvider
    {
#if !WP8
        IDirect2DProvider Direct2D { get; }
        IDirectWriteProvider DirectWrite { get; }
#endif
        IDirect3DProvider Direct3D { get; }
    }

}