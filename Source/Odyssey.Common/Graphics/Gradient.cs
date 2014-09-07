using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Odyssey.Serialization;
using Odyssey.Utilities.Text;
using SharpDX;

namespace Odyssey.Graphics
{
    public abstract class Gradient : ColorResource, IGradient
    {
        private readonly GradientStopCollection gradientStops;

        public GradientStopCollection GradientStops
        {
            get { return gradientStops; }
        }

        protected Gradient(string name, ColorType type) : base(name, type)
        {
            gradientStops = new GradientStopCollection();
        }

        protected Gradient(string name, IEnumerable<GradientStop> gradientStops, ExtendMode extendMode, ColorType type)
            : this(name, type)
        {
            this.gradientStops.AddRange(gradientStops);
            this.gradientStops.ExtendMode = extendMode;
        }
        
        protected override void OnReadXml(XmlDeserializationEventArgs e)
        {
            base.OnReadXml(e);
            var reader = e.XmlReader;
            reader.ReadStartElement();
            while (reader.IsStartElement("GradientStop"))
            {
                Color4 color;
                string colorValue = reader.GetAttribute("Color");
                string colorAsResource = Text.ParseResource(colorValue);
                if (!string.IsNullOrEmpty(colorAsResource))
                    color = e.ResourceProvider.GetResource<SolidColor>(colorAsResource).Color;
                else 
                    color = string.IsNullOrEmpty(colorValue) ? new Color4(0, 0, 0, 0) : Text.DecodeColor4Abgr(colorValue);

                string offset = reader.GetAttribute("Offset");
                var gradientStop = new GradientStop
                {
                    Color = color,
                    Offset = string.IsNullOrEmpty(offset) ? 0 : float.Parse(offset, CultureInfo.InvariantCulture)
                };
                reader.ReadStartElement();
                GradientStops.Add(gradientStop);
            }

            reader.ReadEndElement();
        }

        internal abstract Gradient CopyAs(string newResourceName);
    }
}