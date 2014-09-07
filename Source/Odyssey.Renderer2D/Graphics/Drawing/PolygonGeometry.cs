using System.Linq;
using Odyssey.Engine;
using Odyssey.Geometry.Primitives;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class PolygonGeometry : PathGeometry
    {
        private readonly Polygon polygon;

        private PolygonGeometry(string name, Direct2DDevice device, Polygon polygon)
            : base(name, device)
        {
            this.polygon = polygon;
            Initialize(Resource);
        }

        public Vector2 Center
        {
            get { return polygon.Centroid; }
        }

        public int Sides
        {
            get { return polygon.Count; }
        }

        /// <summary>
        /// <see cref="Polygon"/> casting operator.
        /// </summary>
        /// <param name="from">From the PathGeometry.</param>
        public static implicit operator Polygon(PolygonGeometry from)
        {
            return from == null ? null : from.polygon ?? null;
        }

        public static PolygonGeometry New(string name, Direct2DDevice device, Vector2 center, float circumCircleRadius, int sides, FigureBegin figureBegin)
        {
            var polygon = Polygon.New(center, circumCircleRadius, sides);

            PolygonGeometry polygonGeometry = new PolygonGeometry(name, device, polygon);
            var sink = polygonGeometry.DefineFigure();
            sink.BeginFigure(polygon[0], figureBegin);
            sink.AddLines(polygon.Skip(1));
            sink.EndFigure(FigureEnd.Closed);
            sink.Close();
            return polygonGeometry;
        }
    }
}