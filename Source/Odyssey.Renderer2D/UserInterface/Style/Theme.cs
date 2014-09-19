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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Odyssey.Content;
using Odyssey.Graphics;
using Odyssey.Serialization;
using Odyssey.Utilities.Reflection;

#endregion

namespace Odyssey.UserInterface.Style
{
    [ContentReader(typeof (ThemeReader))]
    public sealed class Theme : ISerializableResource, IResourceProvider
    {
        private const string sResources = "Resources";

        private readonly Dictionary<string, IResource> resources;

        public Theme()
        {
            resources = new Dictionary<string, IResource>();
        }

        public string Name { get; private set; }

        public IEnumerable<IResource> Resources
        {
            get { return resources.Values; }
        }

        [Pure]
        public bool ContainsResource(string resourceName)
        {
            return PreOrderVisit(this).Any(r => r.Name == resourceName);
        }

        public TResource GetResource<TResource>(string resourceName)
            where TResource : class, IResource
        {
            var resource = PreOrderVisit(this).FirstOrDefault(r => r.Name == resourceName);

            if (resource == null)
                throw new ArgumentException(string.Format("Resource '{0}' not found", resourceName));

            TResource resultResource = resource as TResource;
            if (resultResource == null)
                throw new ArgumentException(string.Format("Resource '{0}' of type '{1}' cannot be cast to '{2}'",
                    resourceName, resource, typeof (TResource).Name));
            return resultResource;
        }

        private static IEnumerable<IResource> PreOrderVisit(IResourceProvider root)
        {
            foreach (var resource in root.Resources)
            {
                yield return resource;
                IResourceProvider resourceProvider = resource as IResourceProvider;
                if (resourceProvider != null)
                {
                    foreach (var childResource in PreOrderVisit(resourceProvider))
                        yield return childResource;
                }
            }
        }

        public void AddResource(string resourceName, IResource resource)
        {
            Contract.Requires<ArgumentNullException>(resource != null, "Resource cannot be null");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(resourceName), "Resource name cannot be null");
            if (PreOrderVisit(this).Any(r => r == resource || r.Name == resourceName))
                throw new InvalidOperationException("Resource is already added in the Tree");
            resources.Add(resourceName, resource);
        }

        public void AddResources(IEnumerable<IResource> resources)
        {
            foreach (var resource in resources)
                AddResource(resource.Name, resource);
        }

        #region ISerializableResource

        public void SerializeXml(IResourceProvider resourceProvider, XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }

        public void DeserializeXml(IResourceProvider resourceProvider, XmlReader xmlReader)
        {
            Name = xmlReader.LocalName;
            xmlReader.ReadStartElement();
            while (xmlReader.IsStartElement())
            {
                if (xmlReader.Name == sResources)
                {
                    ParseResources(xmlReader);
                }
                while (xmlReader.IsStartElement())
                {
                    string name = xmlReader.LocalName;
                    switch (name)
                    {
                        case "ControlStyle":
                            ParseControlStyle(xmlReader);
                            break;

                        case "TextStyle":
                            ParseTextStyle(xmlReader);
                            break;

                        default:
                            throw new InvalidOperationException(string.Format("Element '{0}' not recognized", name));
                    }
                }
            }
        }

        private void ParseResources(XmlReader reader)
        {
            reader.ReadStartElement(sResources);
            while (reader.IsStartElement())
            {
                string name = reader.Name;

                string typeName = String.Format("Odyssey.Graphics.{0}, Odyssey.Common", name);

                ColorResource colorResource;
                string resourceName = reader.GetAttribute(ReflectionHelper.GetPropertyName((Theme t) => t.Name));
                try
                {
                    colorResource = (ColorResource) Activator.CreateInstance(Type.GetType(typeName));
                }
                catch (ArgumentNullException ex)
                {
                    throw new InvalidOperationException(String.Format("Type '{0}' is not a valid Resource", typeName));
                }
                colorResource.DeserializeXml(this, reader);
                AddResource(resourceName, colorResource);
            }
            reader.ReadEndElement();
        }

        private void ParseControlStyle(XmlReader reader)
        {
            var style = new ControlStyle();
            AddResource(reader.GetAttribute(ReflectionHelper.GetPropertyName((ControlStyle c) => c.Name)), style);
            style.DeserializeXml(this, reader);
        }

        private void ParseTextStyle(XmlReader reader)
        {
            var style = new TextStyle();
            AddResource(reader.GetAttribute(ReflectionHelper.GetPropertyName((TextStyle t) => t.Name)), style);
            style.DeserializeXml(this, reader);
        }

        #endregion
    }
}