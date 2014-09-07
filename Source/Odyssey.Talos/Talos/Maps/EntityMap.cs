using Odyssey.Content;
using Odyssey.Talos.Components;
using Odyssey.Talos.Messages;
using Odyssey.Utilities.Collections;
using Odyssey.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Odyssey.Utilities.Text;

namespace Odyssey.Talos.Maps
{
    public sealed class EntityMap : IResourceProvider
    {
        readonly Scene scene;
        private readonly DictionaryMap<long, long, Component> componentsByEntity;
        readonly Dictionary<long, Entity> entities;
        private readonly Dictionary<long, Component> components; 

        public event EventHandler<EntityEventArgs> EntityAdded;
        public event EventHandler<EntityEventArgs> EntityRemoved;
        public event EventHandler<EntityComponentChangedEventArgs> EntityComponentAdded;
        public event EventHandler<EntityComponentChangedEventArgs> EntityComponentRemoved;

        protected Messenger Messenger { get { return scene.Messenger; } }
        public IEnumerable<Entity> Entities { get { return entities.Values; } }
        public IEnumerable<Component> Components { get { return components.Values; } }
        public string Name { get; private set; }

        public EntityMap(Scene scene)
        {
            Name = string.Format("{0}.{1}", scene.Name, GetType().Name);
            this.scene = scene;
            entities = new Dictionary<long, Entity>();
            components = new Dictionary<long, Component>();
            componentsByEntity = new DictionaryMap<long, long, Component>();
        }

        private void RaiseEvent<T>(EventHandler<T> handler, object sender, T args)
         where T : EventArgs
        {
            if (handler != null)
                handler(sender, args);
        }

        private void OnEntityAdded(EntityEventArgs args)
        {
            RaiseEvent(EntityAdded, this, args);
        }

        private void OnEntityRemoved(EntityEventArgs args)
        {
            RaiseEvent(EntityRemoved, this, args);
        }

        private void OnEntityComponentRemoved(EntityComponentChangedEventArgs args)
        {
            RaiseEvent(EntityComponentRemoved, this, args);
        }

        private void OnEntityComponentAdded(EntityComponentChangedEventArgs args)
        {
            RaiseEvent(EntityComponentAdded, this, args);
        }

        public void AddEntity(Entity entity)
        {
            if (entity.Scene != null && entity.Scene != scene)
            {
                RemoveEntity(entity);
                LogEvent.Engine.Warning("Entity was already registered in another scene.");
            }
            componentsByEntity.DefineNew(entity.Id);
            entities.Add(entity.Id, entity);
            OnEntityAdded(new EntityEventArgs(entity));
        }

        public void RemoveEntity(Entity entity)
        {
            if (entity.Scene != null && entity.Scene != scene)
            {
                RemoveEntity(entity);
                LogEvent.Engine.Warning("Entity was already registered in another scene.");
            }
            componentsByEntity.Remove(entity.Id);
            entities.Remove(entity.Id);
            OnEntityRemoved(new EntityEventArgs(entity));
        }

        public Entity SelectEntity(long id)
        {
            return entities[id];
        }

        public int Count
        {
            get { return entities.Count; }
        }

        public bool ContainsEntity(Entity entity)
        {
            return entities.ContainsKey(entity.Id);
        }

        public bool ContainsEntity(string name)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(name), "name");
            return entities.Any(kvp => string.Equals(kvp.Value.Name, name));
        }

        public Entity GetEntity(long entityId)
        {
            return entities[entityId];
        }

        public Entity GetEntity(string name)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(name), "name");
            return entities.First(kvp => string.Equals(kvp.Value.Name, name)).Value;
        }

        public void Unload()
        {
            foreach (IDisposable disposableComponent in components.Values.OfType<IDisposable>())
                disposableComponent.Dispose();
        }

        public bool Validate()
        {
            bool test = true;
            foreach (Entity entity in entities.Values)
            {
                bool result = entity.Validate();
                if (!result)
                    test = false;
            }
            return test;
        }

        public IEnumerable<Entity> SelectChildren(Entity parent)
        {
            Contract.Requires<ArgumentNullException>(parent!=null, "parent");
            var childEntities = from e in Entities
                where e.ContainsComponent<ParentComponent>()
                let cParent = e.GetComponent<ParentComponent>()
                where cParent.Parent == parent
                select e;
            return childEntities;
        }

        public IEnumerable<Entity> PreOrderVisit(Entity root)
        {
            foreach (var entity in SelectChildren(root))
            {
                yield return entity;
                foreach (var childEntity in SelectChildren(entity))
                    yield return childEntity;
            }
        }

        public Entity FindChild(Entity parent, string name)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(name), "name");
            return PreOrderVisit(parent).FirstOrDefault(e => string.Equals(e.Name, name));
        }

        public IEnumerable<TComponent> SelectComponents<TComponent>()
            where TComponent : Component
        {
            return components.Values.OfType<TComponent>();
        }

        public IEnumerable<Component> SelectEntityComponents(Entity entity)
        {
            return componentsByEntity[entity.Id].Values;
        }

        public TComponent GetEntityComponent<TComponent>(Entity entity, long keyPart)
            where TComponent : Component
        {
            return (TComponent)componentsByEntity[entity.Id][keyPart];
        }

        public bool TryGetEntityComponent<TComponent>(Entity entity, long keyPart, out TComponent component)
            where TComponent : Component
        {
            Component componentTemp;
            bool test = componentsByEntity[entity.Id].TryGetValue(keyPart, out componentTemp);
            component = (TComponent) componentTemp;
            return test;
        }

        public int CountEntities(Func<Entity, bool> function)
        {
            return entities.Count(kvp => function(kvp.Value));
        }

        public bool EntityHasComponent(Entity entity, long keyPart)
        {
            return componentsByEntity[entity.Id].ContainsKey(keyPart);
        }

        public bool EntityHasComponent(string entityName, string componentType)
        {
            return ContainsEntity(entityName) && GetEntity(entityName).ContainsComponent(componentType);
        }

        public void AddComponentToEntity(Component component, Entity entity)
        {
            if (!components.ContainsKey(component.Id))
            {
                componentsByEntity[entity.Id].Add(component.KeyPart, component);
                components.Add(component.Id, component);
                component.Owner = entity;
            }
            OnEntityComponentAdded(new EntityComponentChangedEventArgs(entity, component));
        }

        public void RemoveComponentFromEntity(Component component, Entity entity)
        {
            componentsByEntity[entity.Id].Remove(component.KeyPart);
            components.Remove(component.Id);
            OnEntityComponentRemoved(new EntityComponentChangedEventArgs(entity, component));
        }

        #region IResourceProvider

        bool IResourceProvider.ContainsResource(string resourceName)
        {
            string resourceArray;
            string index;
            bool isArray = Text.IsExpressionArray(resourceName, out resourceArray, out index);

            return isArray
                ? ContainsEntity(resourceArray) && ((IResourceProvider) GetEntity(resourceArray)).ContainsResource(index)
                : ContainsEntity(resourceName);
        }

        TResource IResourceProvider.GetResource<TResource>(string resourceName)
        {
            string resourceArray;
            string index;
            bool isArray = Text.IsExpressionArray(resourceName, out resourceArray, out index);
            return isArray
                ? ((IResourceProvider) GetEntity(resourceArray)).GetResource<TResource>(index)
                : GetEntity(resourceName) as TResource;
        }

        IEnumerable<IResource> IResourceProvider.Resources
        {
            get { return Entities; }
        }

        #endregion

    }
}
