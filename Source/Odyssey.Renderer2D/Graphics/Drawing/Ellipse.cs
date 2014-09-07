using System;
using Odyssey.UserInterface.Controls;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class Ellipse : Shape
    {
        private SharpDX.Direct2D1.Ellipse ellipse;

        public override bool Contains(Vector2 cursorLocation)
        {
            return BoundingRectangle.Contains(cursorLocation);
        }

        public override void Render()
        {
            if (Fill != null)
                Device.FillEllipse(this,Fill);
            if (Stroke != null)
                Device.DrawEllipse(this, Stroke);
        }

        protected override void OnPositionChanged(EventArgs e)
        {
            base.OnPositionChanged(e);
            ellipse.Point = new Vector2(BoundingRectangle.X, BoundingRectangle.Y);
        }

        protected override void OnLayoutUpdated(EventArgs e)
        {
            base.OnLayoutUpdated(e);
            ellipse = new SharpDX.Direct2D1.Ellipse(BoundingRectangle.Center, BoundingRectangle.Width / 2, BoundingRectangle.Height / 2);
        }

        /// <summary>
        /// <see cref="SharpDX.Direct2D1.Ellipse"/> casting operator.
        /// </summary>
        /// <param name="from">From the Texture1D.</param>
        public static implicit operator SharpDX.Direct2D1.Ellipse(Ellipse from)
        {
            return from == null ? default(SharpDX.Direct2D1.Ellipse) : from.ellipse;
        }
    }
}