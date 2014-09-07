using System.Collections.Generic;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class FillPolylineInstruction : IDesignerInstruction
    {
        private readonly IEnumerable<Vector2> points;
        private readonly float lineWidth;

        public FillPolylineInstruction(IEnumerable<Vector2> points, float lineWidth)
        {
            this.points = points;
            this.lineWidth = lineWidth;
        }

        public void Execute(Designer designer)
        {
            designer.FillPolyline(points, lineWidth);
        }
    }
}
