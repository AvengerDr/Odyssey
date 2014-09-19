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

using Odyssey.Graphics;
using Odyssey.UserInterface.Style;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Brush = Odyssey.Graphics.Brush;
using SolidColorBrush = Odyssey.Graphics.SolidColorBrush;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

#endregion Using Directives

namespace Odyssey.UserInterface.Controls
{
    public class Label : LabelBase
    {
        public Label()
            : base(DefaultTextClass)
        {
        }

        public override void Render()
        {
            DeviceContext context = Device;
            context.DrawText(Text, TextFormat, BoundingRectangle, Foreground);
        }
    }
}