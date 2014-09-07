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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Odyssey.Content;
using Odyssey.Talos.Components;
using Odyssey.Utilities.Logging;
using Odyssey.Utilities.Reflection;
using Odyssey.Utilities.Text;
using SharpYaml.Serialization;

#endregion

namespace Odyssey.Talos
{
    [YamlTag("Entity")]
    [DebuggerDisplay("{Name}, Id={Id}")]
    public sealed class Entity : IEntity, IResourceProvider
    {
        private static int count;
        private readonly long id;
        private long key;
        private Scene scene;

        public Entity() : this("Untitled") {}

        public Entity(string name)
        {
            Name = name;
            id = count++;
            IsEnabled = true;
        }

        public IEnumerable<Entity> Children
        {
            get { return scene.EntityMap.SelectChildren(this); }
        }

        public Scene Scene
        {
            get { return scene; }
        }

        /// <summary>
        /// Returns the progressive id number that identifies this entity instance.
        /// </summary>
        public long Id
        {
            get { return id; }
        }

        public string Name { get; set; }

        public bool IsEnabled { get; set; }

        /// <summary>
        /// Returns the byte value resulting from the set of components associated to this entity instance.
        /// </summary>
        public long Key
        {
            get { return key; }
        }

        public IEnumerable<IComponent> Components
        {
            get { return scene.EntityMap.SelectEntityComponents(this); }
        }

        [Pure]
        public bool ContainsComponent<TComponent>()
            where TComponent : IComponent

        {
            return ContainsComponent(ComponentTypeManager.GetKeyPart<TComponent>());
        }

        public IEnumerable<string> Tags
        {
            get { return scene.TagManager.ContainsEntity(id) ? scene.TagManager[id] : Enumerable.Empty<string>(); }
        }

        public void AddTag(string tag)
        {
            scene.TagManager.AddTagToEntity(id, tag);
        }

        public void RemoveTag(string tag)
        {
            scene.TagManager.RemoveTagFromEntity(id, tag);
        }

        public bool ContainsTag(string tag)
        {
            return scene.TagManager.ContainsTag(id, tag);
        }

        public bool ContainsComponent(long keyPart)
        {
            return Scene.EntityMap.EntityHasComponent(this, keyPart);
        }

        public bool ContainsComponent(string componentType)
        {
            string componentName = string.Format("{0}Component", componentType);
            return Components.Any(c=>string.Equals(c.GetType().Name, componentName));
        }

        public bool TryGetComponent<TComponent>(out TComponent component)
            where TComponent : Component
        {
            return Scene.EntityMap.TryGetEntityComponent(this, ComponentTypeManager.GetKeyPart<TComponent>(), out component);
        }

        public TComponent GetComponent<TComponent>()
            where TComponent : Component
        {
            return GetComponent<TComponent>(ComponentTypeManager.GetKeyPart<TComponent>());
        }

        public TComponent GetComponent<TComponent>(long keyPart)
            where TComponent : Component
        {
            return Scene.EntityMap.GetEntityComponent<TComponent>(this, keyPart);
        }

        public Component GetComponent(string componentType)
        {
            string componentName = string.Format("{0}Component", componentType);
            return Components.First(c => string.Equals(c.GetType().Name, componentName)) as Component;
        }

        public Entity FindChild(string name)
        {
            return scene.EntityMap.FindChild(this, name);
        }

        public bool Validate()
        {
            bool test = true;

            if (!CheckRequiredComponents() || !CheckOptionalComponents())
                return false;

            foreach (var component in Components)
            {
                bool result = component.Validate();
                if (!result)
                    test = false;
            }

            return test;
        }

        public void RegisterComponent(Component component)
        {
            Contract.Requires<ArgumentNullException>(component != null, "component");
            key |= component.KeyPart;
            component.AssignToScene(scene);
            scene.EntityMap.AddComponentToEntity(component, this);
        }

        public void UnregisterComponent(Component component)
        {
            key &= ~component.KeyPart;
            scene.EntityMap.RemoveComponentFromEntity(component, this);
        }

        public void UnregisterComponent<TComponent>()
            where TComponent : Component
        {
            var component = GetComponent<TComponent>();
            UnregisterComponent(component);
        }

        internal void AssignToScene(Scene scene)
        {
            this.scene = scene;
        }

        private bool CheckRequiredComponents()
        {
            bool test = true;
            var attributeTypes = (from component in Components
                from attribute in ReflectionHelper.GetAttributes<RequiredComponentAttribute>(component.GetType())
                select attribute.ComponentType).Distinct();

            var componentTypes = (from otherComponent in Components select otherComponent.GetType()).ToArray();

            foreach (Type type in attributeTypes.Where(type => !componentTypes.Contains(type)))
            {
                test = false;
                LogEvent.Engine.Error("[{0}] is missing [{1}]", Name, type);
            }
            return test;
        }

        private bool CheckOptionalComponents()
        {
            bool test = true;
            var attributeTypes = (from component in Components
                from attribute in ReflectionHelper.GetAttributes<OptionalComponentAttribute>(component.GetType())
                from componentType in attribute.ComponentTypes
                select componentType).Distinct().ToArray();

            if (!attributeTypes.Any())
                return true;

            var componentTypes = (from otherComponent in Components select otherComponent.GetType()).ToArray();

            var intersection = attributeTypes.Intersect(componentTypes);

            if (!intersection.Any())
            {
                test = false;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("[{0}] is missing at least one component between:", Name));
                foreach (Type type in attributeTypes)
                    sb.AppendLine(type.Name);
                LogEvent.Engine.Error(sb.ToString());
            }
            return test;
        }

        #region IResourceProvider

        bool IResourceProvider.ContainsResource(string resourceName)
        {
            string resourceArray;
            string index;
            bool isArray = Text.IsExpressionArray(resourceName, out resourceArray, out index);

            return isArray
                ? ContainsComponent(resourceArray) && ((IResourceProvider) GetComponent(resourceArray)).ContainsResource(index)
                : ContainsComponent(resourceName);

        }

        TResource IResourceProvider.GetResource<TResource>(string resourceName)
        {
            string resourceArray;
            string index;
            bool isArray = Text.IsExpressionArray(resourceName, out resourceArray, out index);
            return isArray
               ? ((IResourceProvider)GetComponent(resourceArray)).GetResource<TResource>(index)
               : GetComponent(resourceName) as TResource;
        }

        IEnumerable<IResource> IResourceProvider.Resources
        {
            get { return Components; }
        }

        #endregion

    }
}