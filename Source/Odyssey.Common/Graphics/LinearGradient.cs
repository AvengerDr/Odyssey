using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;
using Odyssey.Serialization;
using Odyssey.Utilities.Text;
using SharpDX;

namespace Odyssey.Graphics
{
    public sealed class LinearGradient : Gradient, IEquatable<LinearGradient>
    {
        private static int count = 0;
        public Vector2 StartPoint { get; private set; }
        public Vector2 EndPoint { get; private set; }

        public LinearGradient()
            : this(string.Format("{0}{1:D2}", typeof(LinearGradient).Name, ++count), Vector2.Zero, Vector2.UnitX, Enumerable.Empty<GradientStop>())
        {
        }

        public LinearGradient(string name, Vector2 startPoint, Vector2 endPoint, IEnumerable<GradientStop> gradientStops, ExtendMode extendMode = ExtendMode.Clamp, float opacity = 1.0f, bool shared=true)  
            : base(name, gradientStops, extendMode, ColorType.LinearGradient, opacity, shared)
        {
            Contract.Requires<ArgumentNullException>(gradientStops != null, "gradientStops");
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public static LinearGradient Vertical(string name, IEnumerable<GradientStop> gradientStops)
        {
            return new LinearGradient(name, new Vector2(0.5f, 0), new Vector2(0.5f, 1),new GradientStopCollection(gradientStops));
        }

        public static LinearGradient Horizontal(string name, IEnumerable<GradientStop> gradientStops)
        {
            return new LinearGradient(name, new Vector2(0f, 0.5f), new Vector2(1.0f, 0.5f), new GradientStopCollection(gradientStops));
        }

        protected override void OnReadXml(XmlDeserializationEventArgs e)
        {
            var reader = e.XmlReader;
            string sStart = reader.GetAttribute("StartPoint");
            string sEnd = reader.GetAttribute("EndPoint");
            StartPoint = string.IsNullOrEmpty(sStart) ? Vector2.Zero : Text.DecodeFloatVector2(sStart);
            EndPoint = string.IsNullOrEmpty(sEnd) ? Vector2.Zero : Text.DecodeFloatVector2(sEnd);
            base.OnReadXml(e);
        }

        internal override ColorResource CopyAs(string newResourceName, bool shared = true)
        {
            return new LinearGradient(newResourceName, StartPoint, EndPoint, GradientStops, GradientStops.ExtendMode, Opacity, shared);
        }

        #region IEquatable<Gradient>

        public bool Equals(LinearGradient other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StartPoint.Equals(other.StartPoint) && EndPoint.Equals(other.EndPoint) && GradientStops == other.GradientStops;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LinearGradient) obj);
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
    }
}
