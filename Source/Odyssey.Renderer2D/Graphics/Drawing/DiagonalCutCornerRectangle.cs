using System;
using Odyssey.UserInterface.Controls;
using SharpDX;

namespace Odyssey.Graphics.Drawing
{
    public class DiagonalCutCornerRectangle : CutCornerRectangle
    {
        protected override void OnInitializing(EventArgs e)
        {
            base.OnInitializing(e);
            Vector2[] points =
            {
                new Vector2(0, 0),
                new Vector2(Width - CutCornerLength, 0),
                new Vector2(Width, Height - CutCornerLength),
                new Vector2(Width, Height),
                new Vector2(CutCornerLength, Height),
                new Vector2(0, CutCornerLength)
            };

            Shape = PolyLine.New(string.Format("PL.{0}", Name), Device, points, FigureBegin.Filled, FigureEnd.Closed);

        }
    }
}