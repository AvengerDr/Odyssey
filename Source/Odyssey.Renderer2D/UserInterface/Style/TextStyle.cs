using System;
using System.Xml;
using Odyssey.Content;
using Odyssey.Engine;
using Odyssey.Serialization;
using Odyssey.Utilities.Reflection;
using Odyssey.Utilities.Text;
using SharpDX;
using SharpDX.DirectWrite;

namespace Odyssey.UserInterface.Style
{
    public sealed class TextStyle : ISerializableResource, IResource
    {
        public const string Default = "Default";
        public string Name { get; private set; }
        public string Foreground { get; private set; }
        public int Size { get; private set; }
        public string FontFamily { get; private set; }
        public TextAlignment TextAlignment { get; private set; }
        public ParagraphAlignment ParagraphAlignment { get; private set; }
        public FontStyle FontStyle { get; private set; }
        public FontWeight FontWeight { get; private set; }

        #region ISerializableResource
        public void DeserializeXml(IResourceProvider theme, XmlReader xmlReader)
        {
            Name = xmlReader.GetAttribute(ReflectionHelper.GetPropertyName((TextStyle t) => t.Name));
            Foreground = Text.ParseResource(xmlReader.GetAttribute(ReflectionHelper.GetPropertyName((TextStyle t) => t.Foreground)));
            if (!theme.ContainsResource(Foreground))
                throw new InvalidOperationException(string.Format("Resource '{0}' not found", Foreground));
            Size = int.Parse(xmlReader.GetAttribute(ReflectionHelper.GetPropertyName((TextStyle t) => t.Size)));
            FontFamily = xmlReader.GetAttribute(ReflectionHelper.GetPropertyName((TextStyle t) => t.FontFamily));

            string eTextAlignment = xmlReader.GetAttribute(ReflectionHelper.GetPropertyName((TextStyle t) => t.TextAlignment));
            TextAlignment = string.IsNullOrEmpty(eTextAlignment) ? TextAlignment.Leading : Text.ParseEnum<TextAlignment>(eTextAlignment);

            string eParagraphAlignment = xmlReader.GetAttribute(ReflectionHelper.GetPropertyName((TextStyle t) => t.ParagraphAlignment));
            ParagraphAlignment = string.IsNullOrEmpty(eParagraphAlignment) ? ParagraphAlignment.Near : Text.ParseEnum<ParagraphAlignment>(eParagraphAlignment);

            string eFontStyle = xmlReader.GetAttribute(ReflectionHelper.GetPropertyName((TextStyle t) => t.FontStyle));
            FontStyle = string.IsNullOrEmpty(eFontStyle) ? FontStyle.Normal : Text.ParseEnum<FontStyle>(eFontStyle);

            string eFontWeight = xmlReader.GetAttribute(ReflectionHelper.GetPropertyName((TextStyle t) => t.FontWeight));
            FontWeight = string.IsNullOrEmpty(eFontWeight) ? FontWeight.Normal : Text.ParseEnum<FontWeight>(eFontWeight);

            xmlReader.ReadStartElement();
        }

        public void SerializeXml(IResourceProvider theme, XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }
        #endregion

       
    }
}
