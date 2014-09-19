using Point3D = SharpDX.Vector3;

namespace Odyssey.Geometry.Primitives
{
    public class LinearPath : BSpline3D
    {
        public LinearPath() : base(1) {}

        public override Point3D[] Calculate(float alpha)
        {
            if (KnotVector.Count == 0)
                AddKnotVector(CalculateClampedUniformKnotVector(1, Count));

            return base.Calculate(alpha);
        }

        public override Point3D Evaluate(float t)
        {
            if (KnotVector.Count == 0)
                AddKnotVector(CalculateClampedUniformKnotVector(1, Count));

            return base.Evaluate(t);
        }
    }
}
