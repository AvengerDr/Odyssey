using System;
using System.Globalization;
using System.Linq;
using Odyssey.Content;
using Odyssey.Geometry;
using SharpDX;

namespace Odyssey.Animations
{
    public class PathCurve : AnimationCurve<FloatKeyFrame>
    {
        public PathCurve()
        {
            Function = LinearPath;
        }

        public override object Evaluate(float time)
        {
            var function = (IFunction) FunctionOptions;
            FloatKeyFrame start = this.First();
            FloatKeyFrame end = this.Last();
            if (time <= start.Time)
                return function.Evaluate(0);
            else if (time > end.Time)
                return function.Evaluate(1);

            start = this.LastOrDefault(kf => kf.Time < time) ?? start;
            end = this.First(kf => kf.Time > start.Time);
            object result = Function(start, end, time, FunctionOptions);

            return result;
        }

        protected override object DeserializeOptions(string methodName, string options, IResourceProvider resourceProvider)
        {
            switch (methodName)
            {
                case "LinearPath":
                    return resourceProvider.GetResource<IResource>(options);

                default:
                    return base.DeserializeOptions(methodName, options, resourceProvider);
            }
        }


        public static object LinearPath(FloatKeyFrame start, FloatKeyFrame end, float time, object options = null)
        {
            float newValue = Map(start.Time, end.Time, time);
            float t = MathUtil.Lerp(start.Value, end.Value, newValue);

            var function = (IFunction)options;
            return function.Evaluate(t);
        }
    }
}
