using System;
using System.Globalization;
using Odyssey.Content;
using Odyssey.Geometry;
using Odyssey.Talos;
using SharpDX;

namespace Odyssey.Animations
{
    public class FloatCurve : AnimationCurve<FloatKeyFrame>
    {
        public FloatCurve()
        {
            Function = Linear;
        }

        public static object Linear(FloatKeyFrame start, FloatKeyFrame end, float time, object options = null)
        {
            float newValue = Map(start.Time, end.Time, time);

            return MathUtil.Lerp(start.Value, end.Value, newValue);
        }

        protected override object DeserializeOptions(string methodName, string options, IResourceProvider resourceProvider)
        {
            switch (methodName)
            {
                case "SquareWave":
                    return float.Parse(options, CultureInfo.InvariantCulture);

                default:
                    return base.DeserializeOptions(methodName, options, resourceProvider);
            }
        }
    }
}
