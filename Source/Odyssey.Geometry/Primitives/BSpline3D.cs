using Real = System.Single;
using Point3D = SharpDX.Vector3;

namespace Odyssey.Geometry.Primitives
{
    public class BSpline3D : BSplineBase<Point3D>, IFunction
    {
        public BSpline3D(int degree = 2) : base(degree) {}

        public override BSplineBase<Point3D> Derivative()
        {
            Point3D[] newCPs = new Point3D[Count - 1];

            BSpline3D derivative = new BSpline3D(Degree - 1);

            for (int i = 0; i < newCPs.Length; i++)
            {
                newCPs[i] = (this[i + 1] - this[i]) * (Degree / (KnotVector[i + Degree + 1] - KnotVector[i + 1]));
            }

            derivative.AddPoints(newCPs);

            Real[] newKnotVector = new Real[KnotVector.Count - 2];
            for (int i = 0; i < newKnotVector.Length; i++)
            {
                newKnotVector[i] = KnotVector[i + 1];
            }
            derivative.KnotVector.AddRange(newKnotVector);

            return derivative;
        }

        protected override Point3D DeBoor(int k, int i, float t)
        {
            if (k == 0)
            {
                return this[i];
            }
            else
            {
                double alpha = (t - KnotVector[i]) / (KnotVector[i + Degree + 1 - k] - KnotVector[i]);
                return DeBoor(k - 1, i - 1, t) * (1 - (Real)alpha) + DeBoor(k - 1, i, t) * (Real)alpha;
            }
        }
    }
}
