using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Odyssey.Animations;
using Odyssey.Geometry;
using Odyssey.Serialization;
using Odyssey.Utilities.Text;
using SharpDX;

namespace Odyssey.Graphics
{
    public class RadialGradient : Gradient, IEquatable<RadialGradient>
    {
        private static int count;
        private float radiusX;

        public Vector2 Center { get; private set; }

        public Vector2 OriginOffset { get; private set; }

        public float RadiusX { get; set; }

        public float RadiusY { get; set; }

        public RadialGradient()
            : base(string.Format("{0}{1:D2}", typeof(RadialGradient).Name, ++count), ColorType.RadialGradient)
        { }

        public RadialGradient(string name, Vector2 center, Vector2 originOffset, float radiusX, float radiusY, IEnumerable<GradientStop> gradientStops, ExtendMode extendMode= ExtendMode.Clamp) : base(name, gradientStops, extendMode, ColorType.RadialGradient)
        {
            Center = center;
            OriginOffset = originOffset;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public static RadialGradient New(string name, Vector2 center, Vector2 originOffset, float radiusX, float radiusY, IEnumerable<GradientStop> gradientStops)
        {
            return new RadialGradient(name, center, originOffset, radiusX, radiusY, gradientStops);
        }

        public static RadialGradient New(string name, IEnumerable<GradientStop> gradientStops)
        {
            return new RadialGradient(name, Vector2.Zero, Vector2.Zero, 1, 1, new GradientStopCollection(gradientStops));
        }

        protected override void OnReadXml(XmlDeserializationEventArgs e)
        {
            var reader = e.XmlReader;
            string sStart = reader.GetAttribute("Center");
            string sEnd = reader.GetAttribute("OriginOffset");
            string sRadiusX = reader.GetAttribute("RadiusX");
            string sRadiusY = reader.GetAttribute("RadiusY");
            Center = string.IsNullOrEmpty(sStart) ? Vector2.Zero : Text.DecodeFloatVector2(sStart);
            OriginOffset = string.IsNullOrEmpty(sEnd) ? Vector2.Zero : Text.DecodeFloatVector2(sEnd);
            RadiusX = string.IsNullOrEmpty(sRadiusX) ? 0 : float.Parse(sRadiusX, CultureInfo.InvariantCulture);
            RadiusY = string.IsNullOrEmpty(sRadiusY) ? 0 : float.Parse(sRadiusY, CultureInfo.InvariantCulture);
            base.OnReadXml(e);
        }

        #region IEquatable<RadialGradient>
        public bool Equals(RadialGradient other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Center.Equals(other.Center) && OriginOffset.Equals(other.OriginOffset) && RadiusX.Equals(other.RadiusX) && RadiusY.Equals(other.RadiusY) && GradientStops == other.GradientStops;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RadialGradient)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                hashCode = (hashCode * 397) ^ GradientStops.GetHashCode();
                return hashCode;
            }
        }
        #endregion

        internal override Gradient CopyAs(string newResourceName)
        {
            return new RadialGradient(newResourceName, Center, OriginOffset, RadiusX, RadiusY, GradientStops, GradientStops.ExtendMode);
        }
    }
}
