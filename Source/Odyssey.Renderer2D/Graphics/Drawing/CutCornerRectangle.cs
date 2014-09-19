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

#endregion License

#region Using Directives

using System;
using Odyssey.UserInterface.Controls;
using SharpDX;

#endregion Using Directives

namespace Odyssey.Graphics.Drawing
{
    public class CutCornerRectangle : CutCornerRectangleBase
    {
        private PolyLine shape;
        private Matrix3x2 transform;

        protected PolyLine Shape
        {
            get { return shape; }
            set { shape = value; }
        }

        protected new Matrix3x2 Transform
        {
            get { return transform; }
        }

        public override void Render()
        {
            Device.Transform = transform;
            Device.FillGeometry(shape, Fill);
            Device.DrawGeometry(shape, Stroke);
        }

        protected internal override void Measure()
        {
            transform = Matrix3x2.Translation(AbsolutePosition);
        }

        protected override void OnInitializing(EventArgs e)
        {
            base.OnInitializing(e);
            Vector2[] points =
            {
                new Vector2(CutCornerLength, 0),
                new Vector2(Width - CutCornerLength, 0),
                new Vector2(Width, CutCornerLength),
                new Vector2(Width, Height - CutCornerLength),
                new Vector2(Width - CutCornerLength, Height),
                new Vector2(CutCornerLength, Height),
                new Vector2(0, Height - CutCornerLength),
                new Vector2(0, CutCornerLength)
            };

            shape = ToDispose(PolyLine.New(string.Format("PL.{0}", Name), Device, points, FigureBegin.Filled, FigureEnd.Closed));

        }
    }
}