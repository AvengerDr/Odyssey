using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Real = System.Single;
using Point = SharpDX.Vector2;

namespace Odyssey.Geometry.Primitives
{
    public abstract class CurveBase<TPoint>
    {
        private readonly int degree;
        private readonly List<TPoint> controlPoints;
        public TPoint FirstControlPoint { get { return controlPoints[0]; } }
        public TPoint LastControlPoint { get { return controlPoints[controlPoints.Count - 1]; } }

        public int Degree
        {
            get { return degree; }
        }

        public int Count { get { return controlPoints.Count; } }

        protected CurveBase(int degree)
        {
            this.degree = degree;
            controlPoints = new List<TPoint>();
        }

        public abstract TPoint[] Calculate(Real alpha);

        public abstract TPoint Evaluate(Real t);

        public void AddPoint(TPoint point)
        {
            controlPoints.Add(point);
        }

        public void AddPoints(IEnumerable<TPoint> points)
        {
            Contract.Requires<ArgumentNullException>(points!=null, "points");
            controlPoints.AddRange(points);
        }

        public TPoint this[int index]
        {
            get { return controlPoints[index]; }
            set { controlPoints[index] = value; }
        }

    }
}
