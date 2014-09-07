using System;
using System.Collections.Generic;
using Real = System.Single;
using Point = SharpDX.Vector2;

namespace Odyssey.Geometry.Primitives
{
    public class BSpline : CurveBase
    {
        private bool isClamped;
        private readonly List<Real> knotVector;

        public BSpline(int degree = 2) : base(degree)
        {
            knotVector = new List<Real>();
        }

        public override Point Evaluate(Real t)
        {
            int j = WhichInterval(t);
            return DeBoor(Degree, j, t);
        }

        public override Point[] Calculate(Real alpha)
        {
            if (Count < Degree+1)
                throw new InvalidOperationException("Insufficient number of control points");
            if (knotVector.Count ==0)
                throw new InvalidOperationException("Must calculate a knot vector before generating BSpline");
            
            List<Point> points = new List<Point>();

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
                Point p = Evaluate(t);
                points.Add(p);
            }

            if (isClamped)
            {
                points.Insert(0, FirstControlPoint);
                points.Add(LastControlPoint);
            }
            return points.ToArray();
        }

        public void CalculateUniformKnotVector()
        {
            knotVector.Clear();
            float[] x = new float[Count+Degree+1];
            float grad = 1/(Real)(x.Length-1);
            for (int i = 1; i < x.Length; i++)
            {
                x[i] = i*grad;
            }
            knotVector.AddRange(x);
            isClamped = false;
        }

        public BSpline Derivative()
        {
            Point[] newCPs = new Point[Count - 1];

            BSpline derivative = new BSpline(Degree - 1);


            for (int i = 0; i < newCPs.Length; i++)
            {
                newCPs[i] = (this[i + 1] - this[i])*(Degree/(knotVector[i + Degree + 1] - knotVector[i + 1]));
            }

            derivative.AddPoints(newCPs);

            Real[] newKnotVector = new Real[knotVector.Count - 2];
            for (int i = 0; i < newKnotVector.Length; i++)
            {
                newKnotVector[i] = knotVector[i + 1];
            }
            derivative.knotVector.AddRange(newKnotVector);

            return derivative;
        }

        public BSpline Derivative(int iTh)
        {
            BSpline bs = this;
            while (iTh > 0)
            {
                iTh--;
                bs = bs.Derivative();
            }

            return bs;
        }

        public void CalculateClampedUniformKnotVector()
        {
            knotVector.Clear();
            int n = Count - 1;
            int d = Degree + 1;

            float[] x = new float[n + d + 1];
            for (int i = 0; i < x.Length; i++)
            {
                if (i < d)
                    x[i] = 0;
                else if (i > n)
                    x[i] = 1;
                else
                    x[i] = (i-Degree) / (float)(n - Degree + 1);
            }
            knotVector.AddRange(x);
            isClamped = true;
        }

       
        Point DeBoor(int k, int i, Real t)
        {
            if (k == 0)
            {
                return this[i];
            }
            else
            {
                double alpha = (t - knotVector[i])/(knotVector[i + Degree + 1 - k] - knotVector[i]);
                return DeBoor(k - 1, i - 1, t)*(1 - (Real) alpha) + DeBoor(k - 1, i, t)*(Real) alpha;
            }
        }
        
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
