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
using Odyssey.Content;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.Talos.Components;
using Odyssey.Talos.Maps;
using Odyssey.Talos.Messages;
using Odyssey.Talos.Systems;
using Odyssey.Utilities.Logging;
using Odyssey.Utilities.Text;
using SharpDX;
using Component = Odyssey.Talos.Components.Component;
#endregion

namespace Odyssey.Talos
{
    public class Scene : IEntityProvider, IResourceProvider, IUpdateable, IRenderable
    {
        private readonly List<IRenderableSystem> currentlyRenderingSystems;
        private readonly List<IRenderableSystem> renderableSystems;
        private readonly List<IUpdateableSystem> currentlyUpdatingSystems;
        private readonly List<IUpdateableSystem> updateableSystems;
        private readonly EntityMap entityMap;
        private readonly Messenger messenger;
        private readonly SystemMap systemMap;
        private readonly TagManager tagManager;

        public string Name { get; set; }

        public Scene(IServiceRegistry services)
        {
            Contract.Requires<ArgumentNullException>(services != null, "services");
            messenger = new Messenger();
            entityMap = new EntityMap(this);
            systemMap = new SystemMap(this);
            updateableSystems = new List<IUpdateableSystem>();
            renderableSystems = new List<IRenderableSystem>();
            currentlyUpdatingSystems = new List<IUpdateableSystem>();
            currentlyRenderingSystems = new List<IRenderableSystem>();
            Services = services;
            Services.AddService(typeof (IEntityProvider), this);
            Services.AddService(typeof(IResourceProvider), this);
            var content = Services.GetService<IAssetProvider>();
            content.AddMapping("Scene", typeof (Scene));

            entityMap.EntityAdded += systemMap.OnEntityAdded;
            entityMap.EntityRemoved += systemMap.OnEntityRemoved;
            entityMap.EntityComponentAdded += systemMap.OnEntityComponentAdded;
            entityMap.EntityComponentRemoved += systemMap.OnEntityComponentRemoved;

            systemMap.SystemAdded += SystemMapOnSystemAdded;
            systemMap.SystemRemoved += SystemMapOnSystemRemoved;

            tagManager = new TagManager();
        }

        public Scene(IEnumerable<SystemBase> systems, IServiceRegistry services)
            : this(services)
        {
            foreach (SystemBase system in systems)
                AddSystem(system);
        }

        public bool IsDesignMode { get; private set; }

        public IEnumerable<SystemBase> Systems
        {
            get { return systemMap.Systems; }
        }

        internal Messenger Messenger
        {
            get { return messenger; }
        }

        internal SystemMap SystemMap
        {
            get { return systemMap; }
        }

        internal EntityMap EntityMap
        {
            get { return entityMap; }
        }

        internal TagManager TagManager
        {
            get { return tagManager; }
        }

        public IEnumerable<Entity> Entities
        {
            get { return entityMap.Entities; }
        }

        public IServiceRegistry Services { get; private set; }

        public void BeginDesign()
        {
            IsDesignMode = true;
        }

        public bool ContainsEntity(Entity entity)
        {
            return entityMap.ContainsEntity(entity);
        }

        public void EndDesign()
        {
            IsDesignMode = false;
            Validate();
            StartSystems();
        }

        public void Render(ITimeService time)
        {
            lock (renderableSystems)
            {
                foreach (var renderable in renderableSystems)
                    currentlyRenderingSystems.Add(renderable);
            }

            foreach (IRenderableSystem system in currentlyRenderingSystems)
            {
                if (!system.IsEnabled)
                    continue;

                if (system.BeginRender())
                {
                    system.Render(time);
                    //system.EndRender();
                }
            }

            currentlyRenderingSystems.Clear();
        }

        public Entity SelectEntity(long id)
        {
            return entityMap.SelectEntity(id);
        }

        internal Entity FindEntityFromComponent(Component component)
        {
            return (from e in Entities
                where e.ContainsComponent(component.KeyPart)
                let c = e.GetComponent<Component>(component.KeyPart)
                where c.Id == component.Id
                select e).FirstOrDefault();
        }

        public void Unload()
        {
            entityMap.Unload();
            systemMap.Unload();
        }

        public void Update(ITimeService time)
        {
            lock (updateableSystems)
            {
                foreach (var updateable in updateableSystems)
                    currentlyUpdatingSystems.Add(updateable);
            }

            foreach (IUpdateableSystem system in currentlyUpdatingSystems)
            {
                system.BeforeUpdate();

                if (!system.IsEnabled)
                    continue;

                system.Process(time);
                system.AfterUpdate();
            }

            currentlyUpdatingSystems.Clear();
        }

        public void AddEntity(Entity entity)
        {
            entity.AssignToScene(this);
            entityMap.AddEntity(entity);
        }

        public void AddSystem(SystemBase system)
        {
            systemMap.AddSystem(system);
        }

        public Entity CreateEntity(string name)
        {
            Entity entity = new Entity(name);
            AddEntity(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            entityMap.RemoveEntity(entity);
        }

        public void RemoveSystem(SystemBase system)
        {
            systemMap.RemoveSystem(system);
        }

        private void StartSystems()
        {
            foreach (SystemBase system in systemMap.Systems)
                system.Start();
        }

        private void SystemMapOnSystemAdded(object sender, SystemEventArgs args)
        {
            var system = args.Source;

            var updateableSystem = system as IUpdateableSystem;
            if (updateableSystem != null)
                updateableSystems.Add(updateableSystem);

            var renderableSystem = system as IRenderableSystem;
            if (renderableSystem != null)
                renderableSystems.Add(renderableSystem);
        }

        private void SystemMapOnSystemRemoved(object sender, SystemEventArgs args)
        {
            var system = args.Source;

            var updateableSystem = system as IUpdateableSystem;
            if (updateableSystem != null)
            {
                lock (updateableSystems)
                    updateableSystems.Remove(updateableSystem);
            }

            var renderableSystem = system as IRenderableSystem;
            if (renderableSystem != null)
            {
                lock (renderableSystems)
                    renderableSystems.Remove(renderableSystem);
            }
        }

        private void Validate()
        {
            if (entityMap.Count == 0)
                throw new InvalidOperationException("No entities registered to scene");
            if (entityMap.CountEntities(e => e.ContainsComponent(ComponentTypeManager.GetKeyPart<CameraComponent>())) == 0)
                LogEvent.Engine.Warning("No camera registered to scene");
            if (entityMap.CountEntities(e => e.ContainsComponent(ComponentTypeManager.GetKeyPart<PointLightComponent>())) == 0)
                LogEvent.Engine.Warning("No light registered to scene");
            if (!entityMap.Validate())
                throw new InvalidOperationException("Entities are missing required components");
        }

        public Entity this[string entityName]
        {
            get { return entityMap.GetEntity(entityName); }
        }

        #region IResourceProvider

        bool IResourceProvider.ContainsResource(string resourceName)
        {
            IResourceProvider resourceProvider = entityMap;
            return resourceProvider.ContainsResource(resourceName);
        }

        TResource IResourceProvider.GetResource<TResource>(string resourceName)
        {
            IResourceProvider resourceProvider = entityMap;
            return resourceProvider.GetResource<TResource>(resourceName);
        }

        IEnumerable<IResource> IResourceProvider.Resources
        {
            get { return ((IResourceProvider)entityMap).Resources; }
        }

        #endregion
    }
}