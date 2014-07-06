﻿using System;

namespace Odyssey.Graphics
{
    [Flags]
    public enum Borders
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
        All = Top | Bottom | Left | Right,
    }
}
