using SharpDX;

namespace Odyssey.Geometry
{
    public interface IFunction
    {
        Vector3 Evaluate(float t);
    }
}
