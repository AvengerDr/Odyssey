using System;
using System.Globalization;
using System.Xml;
using Odyssey.Content;
using Odyssey.Serialization;
using Odyssey.Utilities.Reflection;

namespace Odyssey.Graphics
{
    public abstract class ColorResource : ISerializableResource, IResource, IColorResource
    {
        private readonly ColorType type;
        private bool shared;

        protected ColorResource(string name, ColorType type, float opacity =1.0f, bool shared = false)
        {
            Name = name;
            this.type = type;
            Opacity = opacity;
            this.shared = shared;
        }

        /// <summary>
        /// Gets the name of this <see cref="ColorResource"/>.
        /// </summary>
        public string Name { get; private set; }

        public ColorType Type
        {
            get { return type; }
        }

        public bool Shared { get { return shared; }}

        public float Opacity { get; set; }

        internal abstract ColorResource CopyAs(string newName, bool shared = true);

        public void SerializeXml(IResourceProvider theme, XmlWriter xmlWriter)
        {
            OnWriteXml(new XmlSerializationEventArgs(theme, xmlWriter));
        }

        public void DeserializeXml(IResourceProvider theme, XmlReader xmlReader)
        {
            OnReadXml(new XmlDeserializationEventArgs(theme, xmlReader));
        }

        protected virtual void OnReadXml(XmlDeserializationEventArgs e)
        {
            var reader = e.XmlReader;
            Name = reader.GetAttribute(ReflectionHelper.GetPropertyName((ColorResource c) => c.Name));
            var sShared = reader.GetAttribute(ReflectionHelper.GetPropertyName((ColorResource c) => c.Shared));
            shared = string.IsNullOrEmpty(sShared) || bool.Parse(sShared);

            string sOpacity = reader.GetAttribute("Opacity");
            Opacity = string.IsNullOrEmpty(sOpacity) ? 1 : float.Parse(sOpacity, CultureInfo.InvariantCulture);

            if (string.IsNullOrEmpty(Name))
            {
                IXmlLineInfo xmlInfo = (IXmlLineInfo)reader;
                throw new InvalidOperationException(string.Format("({0},{1}) 'Name' cannot be null", xmlInfo.LineNumber, xmlInfo.LinePosition));
            }

            // Derived classes will read the other properties
        }

        protected virtual void OnWriteXml(XmlSerializationEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}