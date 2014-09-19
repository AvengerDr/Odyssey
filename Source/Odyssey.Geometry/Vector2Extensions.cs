using Real = System.Single;
using Point = SharpDX.Vector2;

namespace Odyssey.Geometry
{
    internal static class Vector2Extensions
    {
        internal static void Offset(this Point vector, Real xOffset, Real yOffset)
        {
            vector.X += xOffset;
            vector.Y += yOffset;
        }

        /// <summary>
        /// Calculates a vector perpendicular to the source vector.
        /// </summary>
        /// <param name="value">The source vector.</param>
        /// <returns>The resulting perpendicular vector.</returns>
        public static Point Perp(this Point value)
        {
            return new Point(-value.Y, value.X);
        }
    }
}
