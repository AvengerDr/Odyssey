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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using Odyssey.Content;
using Odyssey.Talos.Messages;
using Odyssey.Utilities.Text;
using SharpDX;
using SharpDX.Direct3D11;
using Message = Odyssey.Talos.Messages.Message;

#endregion

namespace Odyssey.Talos.Components
{
    public abstract class Component : IComponent, IResourceProvider
    {
        private static long count;
        private static readonly Dictionary<Type, long> TypeIndex = new Dictionary<Type, long>();
        private readonly ComponentType componentType;
        private readonly long id;
        private Scene scene;

        public event EventHandler<MessageEventArgs> MessageReceveid;

        protected Component(ComponentType type)
        {
            Contract.Requires<ArgumentNullException>(type != null);
            componentType = type;

            Type thisType = GetType();
            if (!TypeIndex.ContainsKey(thisType))
                TypeIndex.Add(thisType, 1);

            id = count++;
            Name = string.Format("{0}{1:00}", thisType.Name, ++TypeIndex[thisType]);
        }

        protected Scene Scene
        {
            get { return scene; }
        }

        public ComponentType ComponentType
        {
            get { return componentType; }
        }

        protected IServiceRegistry Services { get; private set; }

        internal Messenger Messenger { get; set; }

        public Entity Owner { get; internal set; }

        public string Name { get; set; }

        /// <summary>
        /// Returns the progressive id number that identifies this component instance.
        /// </summary>
        public long Id
        {
            get { return id; }
        }

        public long KeyPart
        {
            get { return componentType.KeyPart; }
        }

        public virtual bool Validate()
        {
            return true;
        }

        public void ReceiveMessage(Message message)
        {
            OnReceiveMessage(new MessageEventArgs(message));
        }

        protected virtual void OnReceiveMessage(MessageEventArgs e)
        {
            var handler = MessageReceveid;
            if (handler != null)
                handler(this, e);
        }

        protected void RaisePropertyChanged([CallerMemberName] string property = "")
        {
            if (scene != null && !scene.IsDesignMode)
                Messenger.Send(new PropertyChangeMessage(property, this));
        }

        internal void AssignToScene(Scene scene)
        {
            this.scene = scene;
            Services = scene.Services;
            Messenger = scene.Messenger;
        }

        #region IResourceProvider

        protected virtual bool ContainsResource(string resourceName)
        {
            return false;
        }

        protected virtual TResource GetResource<TResource>(string resourceName)
            where TResource : class, IResource
        {
            return null;
        }

        protected virtual IEnumerable<IResource> Resources
        {
            get { return Enumerable.Empty<IResource>(); }
        }

        bool IResourceProvider.ContainsResource(string resourceName)
        {
            return ContainsResource(resourceName);

        }

        TResource IResourceProvider.GetResource<TResource>(string resourceName)
        {
            return GetResource<TResource>(resourceName);
        }

        IEnumerable<IResource> IResourceProvider.Resources
        {
            get { return Resources; }
        }

        #endregion
    }
}