﻿#region License

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

#endregion License

#region Using Directives

using Odyssey.Geometry.Primitives;
using SharpDX.Mathematics;

#endregion Using Directives

namespace Odyssey.Graphics.Drawing
{
    public abstract class PolygonBase : Shape
    {
        protected abstract Polygon Polygon { get; set; }
        protected PolygonGeometry PolygonGeometry { get; set; }

        public override bool Contains(Vector2 cursorLocation)
        {
            return Polygon.Contains(cursorLocation);
        }

        public override void Render()
        {
            Device.Transform = Transform;
            if (Fill != null)
            {
                Fill.Transform = Matrix3x2.Scaling(Width, Height) * Transform;
                Device.FillGeometry(PolygonGeometry, Fill);
            }
            if (Stroke != null)
                Device.DrawGeometry(PolygonGeometry, Stroke, StrokeThickness);
            Device.Transform = Matrix3x2.Identity;
        }
    }
}