using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class FillRectangleInstruction : IDesignerInstruction
    {
        private readonly RectangleF rectangle;

        public FillRectangleInstruction(RectangleF rectangle)
        {
            this.rectangle = rectangle;
        }

        public void Execute(Designer designer)
        {
            designer.FillRectangle(rectangle);
        }
    }
}
