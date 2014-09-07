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

using SharpDX;
using SharpDX.Collections;
using SharpDX.IO;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion Using Directives

namespace Odyssey.Content
{
    public class ContentManager : IAssetProvider
    {
        private static readonly Dictionary<string, Type> ContentMapper = new Dictionary<string, Type>();

        private readonly Dictionary<string, object> assetLockers;
        private readonly Dictionary<string, object> loadedAssets;

        private readonly Dictionary<Type, IContentReader> registeredContentReaders;
        private readonly List<IResourceResolver> registeredContentResolvers;
        private readonly IServiceRegistry services;
        private string rootDirectory;

        public event EventHandler<AssetsLoadedEventArgs> AssetsLoaded;

        public ContentManager(IServiceRegistry services)
        {
            this.services = services;
            services.AddService(typeof (IAssetProvider), this);

            // Content resolvers
            Resolvers = new ObservableCollection<IResourceResolver>();
            Resolvers.ItemAdded += ContentResolvers_ItemAdded;
            Resolvers.ItemRemoved += ContentResolvers_ItemRemoved;
            registeredContentResolvers = new List<IResourceResolver>();

            // Content readers.
            Readers = new ObservableDictionary<Type, IContentReader>();
            Readers.ItemAdded += ContentReaders_ItemAdded;
            Readers.ItemRemoved += ContentReaders_ItemRemoved;
            registeredContentReaders = new Dictionary<Type, IContentReader>();

            loadedAssets = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            assetLockers = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            // Init ContentManager
            Resolvers.Add(new FileSystemResourceResolver(Global.Assets));
        }

        /// <summary>
        /// Add or remove registered <see cref="IContentReader"/> to this instance.
        /// </summary>
        public ObservableDictionary<Type, IContentReader> Readers { get; private set; }

        /// <summary>
        /// Add or remove registered <see cref="IResourceResolver"/> to this instance.
        /// </summary>
        public ObservableCollection<IResourceResolver> Resolvers { get; private set; }

        /// <summary>
        /// Gets or sets the root directory.
        /// </summary>
        public string RootDirectory
        {
            get { return rootDirectory; }

            set
            {
                if (loadedAssets.Count > 0)
                {
                    throw new InvalidOperationException(
                        "RootDirectory cannot be changed when a ContentManager has already assets loaded");
                }

                rootDirectory = value;
            }
        }

        public IServiceRegistry Services
        {
            get { return services; }
        }

        public void AddMapping(string key, Type type)
        {
            ContentMapper.Add(key, type);
        }

        [Pure]
        public bool Contains(string assetName)
        {
            return loadedAssets.ContainsKey(assetName);
        }

        /// <summary>
        /// Loads an asset that has been processed by the Content Pipeline.  Reference page contains code sample.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName">The asset name </param>
        /// <param name="options">The options to pass to the content reader (null by default).</param>
        /// <returns>``0.</returns>
        /// <exception cref="InvalidOperationException">If the asset was not found from all <see cref="IResourceResolver" />.</exception>
        /// <exception cref="NotSupportedException">If no content reader was suitable to decode the asset.</exception>
        public virtual T Load<T>(string assetName, object options = null)
        {
            object result;

            // Lock loading by asset name, like this, we can have several loading in multithreaded // with a single instance per asset name
            lock (GetAssetLocker(assetName, true))
            {
                // First, try to load the asset from the cache
                lock (loadedAssets)
                {
                    if (loadedAssets.TryGetValue(assetName, out result))
                    {
                        return (T) result;
                    }
                }

                // Else we need to load it from a content resolver disk/zip...etc.
                string assetPath = Path.Combine(rootDirectory ?? string.Empty, assetName);

                // First, resolve the stream for this asset.
                Stream stream = FindStream(assetPath);
                if (stream == null)
                    throw new InvalidOperationException(assetName);

                result = LoadAsset<T>(assetName, stream, options);

                // Cache the loaded assets
                lock (loadedAssets)
                {
                    loadedAssets.Add(assetName, result);
                }
            }

            // We could have an exception, but that's fine, as the user will be able to find why.
            return (T) result;
        }

        /// <summary>
        /// Returns all loaded assets of type T.
        /// </summary>
        /// <typeparam name="T">The asset type to return.</typeparam>
        /// <returns>A sequence of asset of type T.</returns>
        public IEnumerable<T> GetAll<T>()
        {
            var assets = loadedAssets.Values.OfType<T>();
            return assets;
        }

        public void LoadAssetList(string assetListFile)
        {
            var serializer = new Serializer();
            serializer.Settings.RegisterTagMapping("Asset", typeof (AssetIdentifier));
            serializer.Settings.RegisterTagMapping("Assets", typeof (AssetIdentifier[]));
            AssetIdentifier[] assetList;
            using (var nativeStream = new NativeFileStream(assetListFile, NativeFileMode.Open, NativeFileAccess.Read))
                assetList = serializer.Deserialize<AssetIdentifier[]>(nativeStream);

            foreach (AssetIdentifier assetIdentifier in assetList)
            {
                LoadAsset(assetIdentifier);
            }

            OnAssetsLoaded(new AssetsLoadedEventArgs(assetListFile));
        }

        private void OnAssetsLoaded(AssetsLoadedEventArgs e)
        {
            var handler = AssetsLoaded;
            if (handler != null)
                handler(this, e);
        }

        public Type Map(string type)
        {
            return ContentMapper[type];
        }

        public void Store<T>(string assetName, T asset)
        {
            // Lock loading by asset name, like this, we can have several loading in multithreaded // with a single instance per asset name
            lock (GetAssetLocker(assetName, true))
            {
                // Cache the loaded assets
                lock (loadedAssets)
                {
                    loadedAssets.Add(assetName, asset);
                }
            }
        }

        public bool TryGetAsset(string assetName, out object asset)
        {
            loadedAssets.TryGetValue(assetName, out asset);

            return asset != null;
        }

        public void Unload()
        {
            foreach (IDisposable disposable in Readers.OfType<IDisposable>())
            {
                disposable.Dispose();
            }

            if (loadedAssets.Any())
                foreach (IDisposable disposableAssset in loadedAssets.Values.OfType<IDisposable>())
                {
                    disposableAssset.Dispose();
                }
        }

        public void AddResolver(IResourceResolver resolver)
        {
            Resolvers.Add(resolver);
        }

        public IEnumerable<T> SelectAssets<T>()
        {
            return loadedAssets.Values.OfType<T>();
        }

        /// <summary>
        /// Unloads and disposes an asset.
        /// </summary>
        /// <param name="assetName">The asset name</param>
        /// <returns><c>true</c> if the asset exists and was unloaded, <c>false</c> otherwise.</returns>
        public virtual bool Unload<T>(string assetName)
        {
            return Unload(typeof (T), assetName);
        }

        /// <summary>
        /// Unloads and disposes an asset.
        /// </summary>
        /// <param name="assetType">The asset type</param>
        /// <param name="assetName">The asset name</param>
        /// <returns><c>true</c> if the asset exists and was unloaded, <c>false</c> otherwise.</returns>
        public virtual bool Unload(Type assetType, string assetName)
        {
            Contract.Requires<ArgumentNullException>(assetType != null, "assetType");
            Contract.Requires<ArgumentNullException>(assetName != null, "assetName");
            object asset;

            object assetLockerRead = GetAssetLocker(assetName, false);
            if (assetLockerRead == null)
                return false;

            lock (assetLockerRead)
            {
                lock (loadedAssets)
                {
                    if (!loadedAssets.TryGetValue(assetName, out asset))
                        return false;
                    loadedAssets.Remove(assetName);
                }

                lock (assetLockers)
                    assetLockers.Remove(assetName);
            }

            var disposable = asset as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            return true;
        }

        private void ContentReaders_ItemAdded(object sender, ObservableDictionaryEventArgs<Type, IContentReader> e)
        {
            lock (registeredContentReaders)
            {
                registeredContentReaders.Add(e.Key, e.Value);
            }
        }

        private void ContentReaders_ItemRemoved(object sender, ObservableDictionaryEventArgs<Type, IContentReader> e)
        {
            lock (registeredContentReaders)
            {
                registeredContentReaders.Remove(e.Key);
            }
        }

        private void ContentResolvers_ItemAdded(object sender, ObservableCollectionEventArgs<IResourceResolver> e)
        {
            lock (registeredContentResolvers)
            {
                registeredContentResolvers.Add(e.Item);
            }
        }

        private void ContentResolvers_ItemRemoved(object sender, ObservableCollectionEventArgs<IResourceResolver> e)
        {
            lock (registeredContentResolvers)
            {
                registeredContentResolvers.Remove(e.Item);
            }
        }

        private Stream FindStream(string assetName)
        {
            Stream stream = null;
            List<IResourceResolver> lResolvers;
            lock (Resolvers)
            {
                lResolvers = new List<IResourceResolver>(Resolvers);
            }

            if (lResolvers.Count == 0)
                throw new InvalidOperationException("No resolver registered to this content manager");

            foreach (IResourceResolver rResolver in lResolvers)
            {
                stream = rResolver.Resolve(assetName);
                if (stream != null)
                    break;
            }
            return stream;
        }

        private object GetAssetLocker(string assetName, bool create)
        {
            object assetLockerRead;
            lock (assetLockers)
            {
                if (!assetLockers.TryGetValue(assetName, out assetLockerRead) && create)
                {
                    assetLockerRead = new object();
                    assetLockers.Add(assetName, assetLockerRead);
                }
            }
            return assetLockerRead;
        }

        private void LoadAsset(AssetIdentifier asset)
        {
            // Lock loading by asset name, like this, we can have several loading in multithreaded // with a single instance per asset name
            lock (GetAssetLocker(asset.Name, true))
            {
                // Else we need to load it from a content resolver disk/zip...etc.
                string assetPath = Path.Combine(rootDirectory ?? string.Empty, asset.Path);

                // First, resolve the stream for this asset.
                Stream stream = FindStream(assetPath);
                if (stream == null)
                    throw new InvalidOperationException(asset.Name);

                object result = LoadAsset(asset.Name, stream, Map(asset.Type));

                // Cache the loaded assets
                lock (loadedAssets)
                {
                    if (loadedAssets.ContainsKey(asset.Name) && asset.Operation.HasFlag(AssetOperation.Merge))
                        return;

                    loadedAssets.Add(asset.Name, result);
                }
            }
        }

        private object LoadAsset<T>(string assetName, Stream stream, object options = null)
        {
            return LoadAsset(assetName, stream, typeof (T), options);
        }

        private object LoadAsset(string assetName, Stream stream, Type type, object options = null)
        {
            object result;

            var parameters = new ContentReaderParameters
            {
                AssetName = assetName,
                AssetType = type,
                Stream = stream,
                Options = options,
                Services = services
            };

            try
            {
                IContentReader contentReader;
                lock (registeredContentReaders)
                {
                    if (!registeredContentReaders.TryGetValue(type, out contentReader))
                    {
#if WIN8METRO
                        var contentReaderAttribute =
                            SharpDX.Utilities.GetCustomAttribute<ContentReaderAttribute>(type.GetTypeInfo(), true);
#else
                        var contentReaderAttribute = SharpDX.Utilities.GetCustomAttribute<ContentReaderAttribute>(type, true);
#endif

                        if (contentReaderAttribute != null)
                        {
                            contentReader = Activator.CreateInstance(contentReaderAttribute.ContentReaderType) as IContentReader;
                            if (contentReader != null)
                                Readers.Add(type, contentReader);
                        }
                    }
                }

                if (contentReader == null)
                {
                    throw new NotSupportedException(string.Format(
                            "Type [{0}] doesn't provide a ContentReaderAttribute, and there is no registered content reader for it.",
                            type.FullName));
                }

                result = contentReader.ReadContent(this, ref parameters);

                if (result == null)
                {
                    throw new NotSupportedException(string.Format(
                            "Registered ContentReader of type [{0}] fails to load content of type [{1}] from file [{2}].",
                            contentReader.GetType(), type.FullName, assetName));
                }
            }
            finally
            {
                // If we don't need to keep the stream open, then we can close it
                // and make sure that we will close the stream even if there is an exception.
                if (!parameters.KeepStreamOpen)
                    stream.Dispose();
            }

            return result;
        }
    }
}