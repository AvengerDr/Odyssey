#region License

// Copyright © 2013-2014 Avengers UTD - Adalberto L. Simeone
// 
// The Odyssey Engine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License Version 3 as published by
// the Free Software Foundation.
// 
// The Odyssey Engine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details at http://gplv3.fsf.org/

#endregion

#region Using Directives

using SharpDX;

#endregion

namespace Odyssey.Graphics.Drawing
{
    public class Rectangle : Shape
    {
        public override bool Contains(Vector2 cursorLocation)
        {
            return BoundingRectangle.Contains(cursorLocation);
        }

        public override void Render()
        {
            if (Fill != null)
            {
                Fill.Transform = Matrix3x2.Scaling(Width, Height) * Transform;
                Device.FillRectangle(this, Fill);
            }
            if (Stroke!=null)
                Device.DrawRectangle(this, Stroke, StrokeThickness);
        }
    }
}