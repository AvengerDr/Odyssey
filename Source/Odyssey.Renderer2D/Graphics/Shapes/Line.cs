﻿using Odyssey.UserInterface.Controls;
using SharpDX;

namespace Odyssey.Graphics.Shapes
{
    public class Line : Shape
    {
        public Vector2 P0 { get; set; }
        public Vector2 P1 { get; set; }

        public override bool Contains(SharpDX.Vector2 cursorLocation)
        {
            return false;
        }

        public override void Render()
        {
            Device.DrawLine(this, Stroke, StrokeThickness);
        }

    }
}