using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Odyssey.Content;
using Odyssey.Graphics;

namespace Odyssey.UserInterface.Style
{
    [ContentReader(typeof(ThemeReader))]
    public sealed class Theme : IXmlSerializable, IResourceProvider
    {
        const string sControlStyle = "ControlStyle";
        private const string sResources = "Resources";

        private readonly Dictionary<string, IResource> resources;

        public string Name { get; internal set; }

        public IEnumerable<IResource> Resources { get { return resources.Values; } } 

        public Theme()
        {
            resources = new Dictionary<string, IResource>();
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

        static IEnumerable<IResource> PreOrderVisit(IResourceProvider root)
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
            Contract.Requires<ArgumentNullException>(resource!= null, "Resource cannot be null");
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

        #region IXmlSerializable
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Name = reader.LocalName;
            reader.ReadStartElement();
            while (reader.IsStartElement())
            {
                if (reader.Name == sResources)
                {
                    ParseResources(reader);
                }
                while (reader.IsStartElement(sControlStyle))
                {
                    ParseControlStyle(reader);
                }
            }
        }

        void ParseResources(XmlReader reader)
        {
            reader.ReadStartElement(sResources);
            while (reader.IsStartElement())
            {
                string name = reader.Name;

                string typeName = String.Format("Odyssey.Graphics.{0}, Odyssey.Common", name);

                ColorResource colorResource;
                string resourceName = reader["Name"];
                try
                {
                    colorResource = (ColorResource)Activator.CreateInstance(Type.GetType(typeName));
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
            AddResource(reader.GetAttribute("Name"), style);
            style.DeserializeXml(this, reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
