using System;
using Odyssey.Serialization;
using Odyssey.Utilities.Text;
using SharpDX;

namespace Odyssey.Graphics
{
    public sealed class SolidColor : ColorResource, IEquatable<SolidColor>
    {
        private static int count;
        public Color4 Color { get; set; }

        public SolidColor()
            : this(string.Format("{0}{1:D2}", typeof(SolidColor).Name, ++count), Color4.Black)
        { }

        public SolidColor(string name, Color4 color, float opacity = 1.0f) : base(name,  ColorType.SolidColor)
        {
            Color = color;
            Opacity = opacity;
        }

        protected override void OnReadXml(XmlDeserializationEventArgs e)
        {
            base.OnReadXml(e);
            var reader = e.XmlReader;
            string colorValue = reader.GetAttribute("Color");
            Color = Text.DecodeColor4Abgr(colorValue);
            reader.ReadStartElement();
        }

        #region IEquatable<SolidColor>

        public bool Equals(SolidColor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Color.Equals(other.Color);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SolidColor && Equals((SolidColor) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                return hashCode;
            }
        }
        #endregion
    }
}
