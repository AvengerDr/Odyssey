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

using Odyssey.UserInterface.Style;
using SharpDX;

#endregion Using Directives

namespace Odyssey.Graphics.Drawing
{
    public abstract class CutCornerRectangleBase : Shape
    {
        protected CutCornerRectangleBase()
        {
            CutCornerLength = LayoutManager.Units(2f);
        }

        public float CutCornerLength { get; set; }
    }
}