using System;
using System.Globalization;
using System.Xml;
using Odyssey.Content;
using Odyssey.Serialization;

namespace Odyssey.Graphics
{
    public abstract class ColorResource : ISerializableResource, IResource, IColorResource
    {
        private readonly ColorType type;

        protected ColorResource(string name, ColorType type)
        {
            Name = name;
            this.type = type;
            Opacity = 1.0f;
        }

        public string Name { get; private set; }

        public ColorType Type
        {
            get { return type; }
        }

        public float Opacity { get; set; }

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
            Name = reader.GetAttribute("Name");
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