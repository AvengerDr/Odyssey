using System;
using Odyssey.Geometry.Primitives;
using Odyssey.UserInterface.Controls;

namespace Odyssey.Graphics.Drawing
{
    public class Hexagon : PolygonBase
    {
        private const int Sides = 6;
        private PolygonGeometry polygonGeometry;

        protected override Polygon Polygon { get; set; }

        public override void Render()
        {
            Device.FillGeometry(polygonGeometry, Fill);
            Device.DrawGeometry(polygonGeometry, Stroke);
        }

        protected override void OnInitializing(ControlEventArgs e)
        {
            base.OnInitializing(e);
            polygonGeometry = ToDispose(PolygonGeometry.New(string.Format("PL.{0}", Name), Device, BoundingRectangle.Center, BoundingRectangle.Width / 2, Sides,
                FigureBegin.Filled));
        }

        protected override void OnPositionChanged(EventArgs e)
        {
            base.OnPositionChanged(e);
            Polygon = Polygon.New(BoundingRectangle.Center, BoundingRectangle.Width, Sides);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Polygon = Polygon.New(BoundingRectangle.Center, BoundingRectangle.Width, Sides);
        }
    }
}