using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Real = System.Single;
using Point = SharpDX.Vector2;

namespace Odyssey.Geometry.Primitives
{
    public abstract class CurveBase
    {
        private readonly int degree;
        private readonly List<Point> controlPoints;
        public Point FirstControlPoint { get { return controlPoints[0]; } }
        public Point LastControlPoint { get { return controlPoints[controlPoints.Count - 1]; } }

        public int Degree
        {
            get { return degree; }
        }

        public int Count { get { return controlPoints.Count; } }

        protected CurveBase(int degree)
        {
            this.degree = degree;
            controlPoints = new List<Point>();
        }

        public abstract Point[] Calculate(Real alpha);

        public abstract Point Evaluate(Real t);

        public void AddPoint(Point point)
        {
            controlPoints.Add(point);
        }

        public void AddPoints(IEnumerable<Point> points)
        {
            Contract.Requires<ArgumentNullException>(points!=null, "points");
            controlPoints.AddRange(points);
        }

        public Point this[int index]
        {
            get { return controlPoints[index]; }
            set { controlPoints[index] = value; }
        }

    }
}
