using Odyssey.Content;
using Odyssey.Graphics;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Odyssey.Engine
{
    public class Direct2DResource : Component, IResource, IInitializable
    {

        protected Direct2DResource(string name, Direct2DDevice device)
            : base(name)
        {
            Contract.Requires<ArgumentNullException>(device != null, "device");
            Device = device;
        }

        public Direct2DDevice Device { get; internal set; }

        /// <summary>
        /// The attached Direct2D1 resource to this instance.
        /// </summary>
        protected Resource Resource { get; set; }

        /// <summary>
        /// Implicit casting operator to <see cref="SharpDX.Direct2D1.Resource"/>
        /// </summary>
        /// <param name="from">The GraphicsResource to convert from.</param>
        public static implicit operator Resource(Direct2DResource from)
        {
            return from == null ? null : from.Resource;
        }

        public virtual void Initialize()
        {
            Initialize(Resource);
        }

        protected static void UnPin(GCHandle[] handles)
        {
            if (handles != null)
            {
                for (int i = 0; i < handles.Length; i++)
                {
                    if (handles[i].IsAllocated)
                        handles[i].Free();
                }
            }
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            base.Dispose(disposeManagedResources);
            if (disposeManagedResources)
                Resource = null;
        }

        /// <summary>
        /// Initializes the specified device local.
        /// </summary>
        /// <param name="resource">The resource.</param>
        protected virtual void Initialize(Resource resource)
        {
            Resource = ToDispose(resource);
            if (resource != null)
            {
                resource.Tag = this;
            }
            IsInited = true;
        }

        /// <summary>
        /// Called when name changed for this component.
        /// </summary>
        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "Name")
            {
                if (Device.IsDebugMode && Resource != null)
                {
                    Resource.Tag = Name;
                }
            }
        }

        public bool IsInited { get; private set; }

    }

}