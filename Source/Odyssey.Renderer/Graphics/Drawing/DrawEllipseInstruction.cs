using Odyssey.Geometry.Primitives;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class DrawEllipseInstruction : DrawInstruction
    {
        private readonly Ellipse ellipse;

        public Ellipse Ellipse
        {
            get { return ellipse; }
        }

        public DrawEllipseInstruction(Ellipse ellipse, float strokeThickness) : base(strokeThickness)
        {
            this.ellipse = ellipse;
        }

        public override void Execute(Designer designer)
        {
            designer.DrawEllipse(ellipse, StrokeThickness);
        }
    }
}
