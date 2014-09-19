using System;
using Real = System.Single;
using Point = SharpDX.Vector2;

namespace Odyssey.Geometry.Primitives
{
    public struct Line : IEquatable<Line>
    {
        public Point Origin { get; private set; }
        public Point Direction { get; private set; }
        public Point Normal { get; private set; }

        public Real Ax { get; private set; }
        public Real By { get; private set; }
        public Real C { get; private set; }

        public Line(Point origin, Point direction)
            : this()
        {
            Origin = origin;
            Direction = direction;

            Normal = direction.Perp();
            Ax = Normal.X;
            By = Normal.Y;
            C = Point.Dot(-PointAtDistance((Real) 1.0), Normal);
        }

        public Point PointAtDistance(Real distance)
        {
            return Origin + distance*Point.Normalize(Direction);
        }

        public static int DetermineSide(Line line, Point point)
        {
            return DetermineSide(line, line.Normal, point);
        }

        public static int DetermineSide(Line line, Point normal, Point point)
        {
            Real value = Point.Dot(line.Normal, point) + line.C;

            if (Math.Abs(value) < MathHelper.Epsilon)
                return 0;
            else if (value < 0)
                return -1;
            else //if (value > 0)
                return 1;
        }

        public static Line FromTwoPoints(Point point1, Point point2)
        {
            return new Line(point1, point2 - point1);
        }

        #region Equality

        public bool Equals(Line other)
        {
            return other.Origin.Equals(Origin) && other.Direction.Equals(Direction);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Line)) return false;
            return Equals((Line) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Origin.GetHashCode()*397) ^ Direction.GetHashCode();
            }
        }

        public static bool operator ==(Line left, Line right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Line left, Line right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}