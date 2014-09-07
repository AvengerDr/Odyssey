using System;
using SharpDX;

namespace Odyssey.Animations
{
    public class QuaternionCurve : AnimationCurve<QuaternionKeyFrame>
    {
        public QuaternionCurve()
        {
            Function = Slerp;
        }

        public static object Slerp(QuaternionKeyFrame start, QuaternionKeyFrame end, float time, object options = null)
        {
            float newValue = Map(start.Time, end.Time, time);

            return Quaternion.Slerp(start.Value, end.Value, newValue);
        }
    }
}
