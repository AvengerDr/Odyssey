using Odyssey.UserInterface.Style;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public interface IShape 
    {
        RectangleF BoundingRectangle { get; }

        Brush Fill { get; }

        Brush Stroke { get; }

        float StrokeThickness { get; set; }

        void Render();
    }
}