﻿using Odyssey.Graphics.Drawing;
using SharpDX.Mathematics;

namespace Odyssey.UserInterface.Controls
{
    public class Border : ContentControl
    {
        protected const string ControlTag = "Border";

        public Border() : base(ControlTag, ControlTag)
        {
        }

        public override void Render()
        {
            foreach (IShape shape in VisualState)
                shape.Render();
            base.Render();
        }
    }
}
