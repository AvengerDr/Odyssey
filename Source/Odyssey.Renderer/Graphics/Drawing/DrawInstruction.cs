namespace Odyssey.Graphics.Drawing
{
    public abstract class DrawInstruction : IDesignerInstruction
    {
        private readonly float strokeThickness;

        public float StrokeThickness {get { return strokeThickness; }}

        protected DrawInstruction(float strokeThickness)
        {
            this.strokeThickness = strokeThickness;
        }

        public abstract void Execute(Designer designer);
    }
}
