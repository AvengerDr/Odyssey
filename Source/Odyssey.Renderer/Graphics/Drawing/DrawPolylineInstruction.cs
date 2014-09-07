using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class DrawPolylineInstruction : DrawInstruction
    {
        private readonly IEnumerable<Vector2> points;
        private readonly float lineWidth;

        public DrawPolylineInstruction(IEnumerable<Vector2> points, float lineWidth, float strokeThickness) : base(strokeThickness)
        {
            this.points = points;
            this.lineWidth = lineWidth;
        }

        public override void Execute(Designer designer)
        {
            designer.DrawPolyline(points, lineWidth, StrokeThickness);
        }
    }
}
