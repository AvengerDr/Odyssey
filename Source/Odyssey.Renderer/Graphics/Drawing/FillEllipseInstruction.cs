using Odyssey.Geometry.Primitives;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class FillEllipseInstruction : IDesignerInstruction
    {
        private readonly Ellipse ellipse;

        public Ellipse Ellipse
        {
            get { return ellipse; }
        }

        public FillEllipseInstruction(Ellipse ellipse)
        {
            this.ellipse = ellipse;
        }

        public void Execute(Designer designer)
        {
            designer.FillEllipse(ellipse);
        }
    }
}
