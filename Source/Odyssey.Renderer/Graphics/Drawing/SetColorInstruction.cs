namespace Odyssey.Graphics.Drawing
{
    public class SetColorInstruction : IDesignerInstruction
    {
        private readonly IColorResource color;

        public IColorResource Color
        {
            get { return color; }
        }

        public SetColorInstruction(IColorResource color)
        {
            this.color = color;
        }

        public void Execute(Designer designer)
        {
            designer.Color = Color;
        }
    }
}
