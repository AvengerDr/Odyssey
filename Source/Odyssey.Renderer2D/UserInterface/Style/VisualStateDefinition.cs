using System;
using System.Linq;
using Odyssey.Animations;
using Odyssey.Content;
using Odyssey.Graphics;
using Odyssey.Graphics.Drawing;
using System.Collections.Generic;
using Odyssey.Serialization;
using Odyssey.UserInterface.Controls;

namespace Odyssey.UserInterface.Style
{
    internal sealed class VisualStateDefinition : ISerializableResource, IResourceProvider
    {
        private readonly List<Shape> shapes;
        private readonly List<Animation> animations;

        public string Name { get; set; }

        public VisualStateDefinition()
        {
            shapes = new List<Shape>();
            animations = new List<Animation>();
        }

        public IEnumerable<Shape> Shapes
        {
            get { return shapes; }
        }

        public IEnumerable<Animation> Animations
        {
            get { return animations; }
        }

        #region ISerializableResource

        public void SerializeXml(IResourceProvider resourceProvider, System.Xml.XmlWriter xmlWriter)
        {
            throw new System.NotImplementedException();
        }

        public void DeserializeXml(IResourceProvider resourceProvider, System.Xml.XmlReader xmlReader)
        {
            const string sShapes = "Shapes";
            const string sAnimation = "Animation";
            const string sAnimations = "Animations";

            Name = xmlReader.GetAttribute("Name");

            if (!xmlReader.ReadToDescendant(sShapes))
                throw new InvalidOperationException(String.Format("{0}' element not found", sShapes));

            // Deserialize shapes
            xmlReader.ReadStartElement();

            while (xmlReader.IsStartElement())
            {
                string typeName = string.Format("Odyssey.Graphics.Drawing.{0}", xmlReader.Name);
                try
                {
                    var shape = (Shape) Activator.CreateInstance(Type.GetType(typeName));
                    shape.DeserializeXml(resourceProvider, xmlReader);
                    shapes.Add(shape);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException(String.Format("Type '{0}' is not a valid Shape", typeName));
                }
                xmlReader.Read();
            }
            xmlReader.ReadEndElement();

            if (!xmlReader.IsStartElement(sAnimations))
            {
                xmlReader.ReadEndElement();
                return;
            }

            // Deserialize animations
            xmlReader.ReadStartElement(sAnimations);
            while (xmlReader.IsStartElement(sAnimation))
            {
                string sState = xmlReader.GetAttribute("Name");
                ControlStatus cStatus;
                if (!Enum.TryParse(sState, out cStatus))
                    throw new InvalidOperationException(string.Format("'{0}' is not a valid state", sState));

                var animation = new Animation();
                animation.DeserializeXml(resourceProvider, xmlReader);
                animations.Add(animation);
            }
            xmlReader.ReadEndElement();

            xmlReader.ReadEndElement();

        }

        #endregion

        #region IResourceProvider
        bool IResourceProvider.ContainsResource(string resourceName)
        {
           return Shapes.Any(s => s.Name == resourceName) || Animations.Any(a => a.Name == resourceName);
        }

        TResource IResourceProvider.GetResource<TResource>(string resourceName)
        {
            return Shapes.FirstOrDefault(s => s.Name == resourceName) as TResource ??
                   Animations.FirstOrDefault(s => s.Name == resourceName) as TResource;
        }

        IEnumerable<IResource> IResourceProvider.Resources
        {
            get
            {
                foreach (var shape in shapes)
                {
                    yield return shape;
                }
                foreach (var animation in animations)
                {
                    yield return animation;
                }
            }
        } 
        #endregion
    }
}
