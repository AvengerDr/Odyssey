using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class DrawRectangleInstruction : DrawInstruction
    {
        private readonly RectangleF rectangle;

        public RectangleF Rectangle
        {
            get { return rectangle; }
        }

        public DrawRectangleInstruction(RectangleF rectangle, float strokeThickness)
            : base(strokeThickness)
        {
            this.rectangle = rectangle;
        }

        public override void Execute(Designer designer)
        {
            designer.DrawRectangle(rectangle, StrokeThickness);
        }
    }
}
