using SharpDX;

namespace Odyssey.Graphics
{
    public interface IGradient : IColorResource
    {
        ColorType Type { get; }
        GradientStopCollection GradientStops { get; }
    }

}