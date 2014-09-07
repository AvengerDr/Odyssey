using Odyssey.Graphics.Drawing;
using SharpDX;

namespace Odyssey.UserInterface.Controls
{
    public class Border : ContentControl
    {
        protected const string ControlTag = "Border";

        public Border() : base(ControlTag, ControlTag)
        {
        }

        public override bool Contains(Vector2 cursorLocation)
        {
            return BoundingRectangle.Contains(cursorLocation);
        }

        public override void Render()
        {
            foreach (IShape shape in VisualState)
                shape.Render();
            base.Render();
        }
    }
}
