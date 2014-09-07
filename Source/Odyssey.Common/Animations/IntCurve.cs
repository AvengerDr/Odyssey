using SharpDX;

namespace Odyssey.Animations
{
    public class IntCurve : AnimationCurve<IntKeyFrame>
    {
        public IntCurve()
        {
            Function = Linear;
        }


        public static object Linear(IntKeyFrame start, IntKeyFrame end, float time, object options = null)
        {
            float newValue = Map(start.Time, end.Time, time);
            return (int)MathUtil.Lerp(start.Value, end.Value, newValue);
        }
    }
}
