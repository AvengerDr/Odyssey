#region Using Directives

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Odyssey.Content;
using Odyssey.Engine;
using Odyssey.Graphics;
using SharpDX;
using System;
using System.Linq;
using SharpDX.DirectWrite;
using Font = Odyssey.Content.Font;
using Path = System.IO.Path;

#endregion Using Directives

namespace Odyssey.UserInterface.Style
{
    public class StyleManager : Component, IStyleService
    {
        private struct ResourceDescription
        {
            public IResource Value { get; private set; }
            public bool IsShared { get; private set; }

            public ResourceDescription(bool isShared, IResource value) : this()
            {
                IsShared = isShared;
                Value = value;
            }
        }

        private readonly IAssetProvider content;
        private readonly Dictionary<string, IResource> uniqueResources;
        private readonly Dictionary<string, ResourceDescription> sharedResources;
        private FontCollection fontCollection;
        private NativeFontLoader fontLoader;
        private readonly IServiceRegistry services;
        public FontCollection FontCollection { get { return fontCollection; } }

        public StyleManager(IServiceRegistry services)
        {
            this.services = services;
            content = services.GetService<IAssetProvider>();

            content.AddMapping("Theme", typeof (Theme));
            content.AddMapping("Font", typeof(Font));

            content.AssetsLoaded += InitializeFontCollection;
            sharedResources = new Dictionary<string, ResourceDescription>();
            uniqueResources= new Dictionary<string, IResource>();
        }

        void InitializeFontCollection(object sender, AssetsLoadedEventArgs e)
        {
            var assetListName = Path.GetFileNameWithoutExtension(e.AssetListPath);
            if (!string.Equals(assetListName, "fonts"))
                return;

            var d2DService = services.GetService<IDirect2DService>();
            d2DService.DeviceDisposing += (s, args) => Unload();

            fontLoader = new NativeFontLoader(services);
            fontCollection = new FontCollection(d2DService.Direct2DDevice, fontLoader, fontLoader.Key);
            content.AssetsLoaded -= InitializeFontCollection;
        }

        [Pure]
        public bool ContainsResource(string resourceName)
        {
            return sharedResources.ContainsKey(resourceName);
        }

        public void AddResource(IResource resource, bool shared = true)
        {
            Contract.Requires<ArgumentException>(!ContainsResource(resource.Name), "A resource with the same name is already in the collection");

            sharedResources.Add(resource.Name, new ResourceDescription(shared, resource));

            var disposableResource = resource as IDisposable;
            if (disposableResource != null)
                ToDispose(disposableResource);

            var initializableResource = resource as IInitializable;
            if (initializableResource != null && !initializableResource.IsInited)
                initializableResource.Initialize();
        }

        public Theme GetTheme(string themeName)
        {
            return content.Load<Theme>(themeName);
        }

        public TResource GetResource<TResource>(string resourceName) where TResource : class, IResource
        {
            if (!ContainsResource(resourceName)) 
                throw new ArgumentException(string.Format("Resource '{0}' not found", resourceName));

            var resource = sharedResources[resourceName].Value;
            TResource resultResource = resource as TResource;
            if (resultResource == null)
                throw new ArgumentException(string.Format("Resource '{0}' of type '{1}' cannot be cast to '{2}'",
                    resourceName, resource, typeof(TResource).Name));
            return resultResource;
        }

        public bool TryGetResource<TResource>(string resourceName, bool shared,out TResource resource) 
            where TResource : class, IResource
        {
            resource = null;
            if (!ContainsResource(resourceName)) return false;

            var resourceDescription = sharedResources[resourceName];
            resource = resourceDescription.IsShared == shared ? (TResource)resourceDescription.Value : null;
            return resource != null;
        }

        public IEnumerable<TResource> GetResources<TResource>() where TResource : class, IResource
        {
            return (from rd in sharedResources.Values select rd.Value).OfType<TResource>();
        }

        public IEnumerable<IResource> Resources
        {
            get { throw new NotImplementedException(); }
        }

        Brush CreateColorResource(Direct2DDevice device, ColorResource colorResource)
        {
            var resource = Brush.FromColorResource(device, colorResource);
            AddResource(resource, colorResource.Shared);
            return resource;
        }

        public Brush CreateOrRetrieveColorResource(ColorResource colorResource, bool shared = true)
        {
            var device = services.GetService<IDirect2DService>().Direct2DDevice;

            Brush result;
            if (TryGetResource(colorResource.Name, shared, out result))
                return result;

            var solidColor = colorResource as SolidColor;
            if (solidColor != null)
            {
                var brushes = GetResources<SolidColorBrush>();
                result = brushes.FirstOrDefault(b => b.Color.Equals(solidColor.Color) && b.Shared == shared);
            }
            return result ?? CreateColorResource(device, shared ? colorResource : colorResource.CopyAs('u' + colorResource.Name, false));
        }

        TextFormat CreateTextResource(TextStyle textStyle, bool shared)
        {
            var resource = TextFormat.New(services, textStyle);
            AddResource(resource, shared);
            return resource;
        }

        public TextFormat CreateOrRetrieveTextResource(TextStyle textStyle, bool shared = true)
        {
            TextFormat result;
            if (TryGetResource(textStyle.Name, shared, out result))
                return result;

            if (!shared)
                return CreateTextResource(textStyle, false);

            return result ?? CreateTextResource(textStyle, true);
        }

        void Unload()
        {
            // FontCollection and FontLoader have to be unregistered and disposed before
            // the DirectWriteFactory is disposed
            var dwFactory = services.GetService<IDirect2DService>().Direct2DDevice.DirectWriteFactory;
            dwFactory.UnregisterFontCollectionLoader(fontLoader);
            dwFactory.UnregisterFontFileLoader(fontLoader);
            SharpDX.Utilities.Dispose(ref fontCollection);
            SharpDX.Utilities.Dispose(ref fontLoader);
        }

    }
}