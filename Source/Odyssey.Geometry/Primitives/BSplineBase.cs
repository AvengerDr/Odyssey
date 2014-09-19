using System;
using System.Collections.Generic;
using Real = System.Single;
using Point = SharpDX.Vector2;

namespace Odyssey.Geometry.Primitives
{
    public abstract class BSplineBase<TPoint> : CurveBase<TPoint>
    {
        private bool isClamped;
        private readonly List<Real> knotVector;

        protected List<Real> KnotVector { get { return knotVector; } }

        protected BSplineBase(int degree = 2)
            : base(degree)
        {
            knotVector = new List<Real>();
        }

        public override TPoint Evaluate(Real t)
        {
            if (MathHelper.ScalarNearEqual(t, 1.0f) && isClamped)
                return this[Count - 1];
            int j = WhichInterval(t);
            return DeBoor(Degree, j, t);
        }

        public abstract BSplineBase<TPoint> Derivative();

        public BSplineBase<TPoint> Derivative(int iTh)
        {
            var bs = this;
            while (iTh > 0)
            {
                iTh--;
                bs = bs.Derivative();
            }

            return bs;
        }

        public override TPoint[] Calculate(Real alpha)
        {
            if (Count < Degree+1)
                throw new InvalidOperationException("Insufficient number of control TPoints");
            if (knotVector.Count ==0)
                throw new InvalidOperationException("Must calculate a knot vector before generating BSpline");
            
            List<TPoint> TPoints = new List<TPoint>();

            float start;
            float end;

            if (isClamped)
            {
                start = alpha;
                end = 1 - alpha;
            }
            else
            {
                start = Degree * 1/((Real) knotVector.Count - 1);
                end = 1-start;
            }

            for (float t = start; t < end; t += alpha)
            {
                TPoint p = Evaluate(t);
                TPoints.Add(p);
            }

            if (isClamped)
            {
                TPoints.Insert(0, FirstControlPoint);
                TPoints.Add(LastControlPoint);
            }
            return TPoints.ToArray();
        }

        public void AddKnotVector(Real[] knots, bool isClamped = true)
        {
            knotVector.Clear();
            knotVector.AddRange(knots);
            this.isClamped = isClamped;
        }

        public static Real[] CalculateUniformKnotVector(int degree, int cpCount)
        {
            var x = new Real[cpCount + degree + 1];
            Real grad = 1 / (Real)(x.Length - 1);
            for (int i = 1; i < x.Length; i++)
            {
                x[i] = i*grad;
            }
            return x;
        }

        

        public static Real[] CalculateClampedUniformKnotVector(int degree, int cpCount)
        {
            int n = cpCount - 1;
            int d = degree + 1;

            var x = new Real[n + d + 1];
            for (int i = 0; i < x.Length; i++)
            {
                if (i < d)
                    x[i] = 0;
                else if (i > n)
                    x[i] = 1;
                else
                    x[i] = (i - degree) / (Real)(n - degree + 1);
            }
            return x;
        }

        protected abstract TPoint DeBoor(int k, int i, Real t);
        
        int WhichInterval(Real x)
        {
            int ti = knotVector.Count;
            for (int i = 1; i < ti - 1; i++)
            {
                if (x < knotVector[i])
                    return (i - 1);
                else if (MathHelper.ScalarNearEqual(x, knotVector[ti - 1]))
                    return (ti - 1);
            }
            return -1;
        }
    }
}
