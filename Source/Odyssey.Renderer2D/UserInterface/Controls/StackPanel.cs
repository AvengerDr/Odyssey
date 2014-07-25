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

using Odyssey.Graphics;
using Odyssey.Graphics.Shapes;
using SharpDX;
using Rectangle = Odyssey.Graphics.Shapes.Rectangle;

#endregion Using Directives

namespace Odyssey.UserInterface.Controls
{
    public class StackPanel : StackPanelBase
    {
        public override bool Contains(Vector2 cursorLocation)
        {
            return BoundingRectangle.Contains(cursorLocation);
        }
        protected override void OnInitializing(ControlEventArgs e)
        {
            base.OnInitializing(e);

            Rectangle rEnabled =
                ToDispose(Shape.FromControl<Rectangle>(this, string.Format("{0}_{1}_rectangle", Name, ControlStatus.Enabled)));
            ShapeMap.Add(ControlStatus.Enabled, new[] {rEnabled});
        }
    }
}