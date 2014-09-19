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
using System.Globalization;
using System.Linq;
using System.Xml;
using Odyssey.Animations;
using Odyssey.Content;
using Odyssey.Graphics;
using Odyssey.Graphics.Drawing;
using Odyssey.Serialization;
using Odyssey.UserInterface.Controls;

#endregion

namespace Odyssey.UserInterface.Style
{
    public sealed class ControlStyle : ISerializableResource, IResourceProvider
    {
        public const string Empty = "Empty";
        internal const string Error = "Error";

        private VisualStateDefinition visualStateDefinition;

        public float Width { get; private set; }
        public float Height { get; private set; }
        public Thickness Margin { get; private set; }
        public Thickness Padding { get; private set; }
        public string TextStyleClass { get; private set; }

        public string Name { get; private set; }

        public VisualState CreateVisualState(Control control)
        {
            return VisualState.GenerateVisualStateForControl(control, visualStateDefinition);
        }

        #region ISerializableResource
        public void DeserializeXml(IResourceProvider theme, XmlReader xmlReader)
        {
            const string sVisualState = "VisualState";
            Name = xmlReader.GetAttribute("Name");

            string margin = xmlReader.GetAttribute("Margin");
            Margin = String.IsNullOrEmpty(margin) ? Thickness.Empty : StyleHelper.DecodeThickness(margin);

            string textStyleClass = xmlReader.GetAttribute("TextStyle");
            TextStyleClass = String.IsNullOrEmpty(textStyleClass) ? "Default" : textStyleClass;

            string padding = xmlReader.GetAttribute("Padding");
            Padding = String.IsNullOrEmpty(padding) ? Thickness.Empty : StyleHelper.DecodeThickness(padding);

            string width = xmlReader.GetAttribute("Width");
            string height = xmlReader.GetAttribute("Height");
            Width = String.IsNullOrEmpty(width) ? 0 : Single.Parse(width, CultureInfo.InvariantCulture);
            Height = String.IsNullOrEmpty(height) ? 0 : Single.Parse(height, CultureInfo.InvariantCulture);

            if (!xmlReader.ReadToDescendant(sVisualState))
                throw new InvalidOperationException(String.Format("{0}' element not found", sVisualState));

            visualStateDefinition = new VisualStateDefinition();
            visualStateDefinition.DeserializeXml(theme, xmlReader);
            xmlReader.ReadEndElement();
        }

        public void SerializeXml(IResourceProvider theme, XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        } 
        #endregion

        #region IResourceProvider
        bool IResourceProvider.ContainsResource(string resourceName)
        {
            return ((IResourceProvider) visualStateDefinition).ContainsResource(resourceName);
        }

        TResource IResourceProvider.GetResource<TResource>(string resourceName)
        {
            return ((IResourceProvider) visualStateDefinition).GetResource<TResource>(resourceName);
        }

        IEnumerable<IResource> IResourceProvider.Resources
        {
            get { return ((IResourceProvider) visualStateDefinition).Resources; }
        } 
        #endregion
        
    }
}