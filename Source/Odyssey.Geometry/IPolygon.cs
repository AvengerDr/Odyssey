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

#endregion

#region Using Directives

using System.Collections.Generic;
using Real = System.Single;
using Point = SharpDX.Mathematics.Vector2;

#endregion

namespace Odyssey.Geometry
{
    public interface IPolygon : IEnumerable<Point>
    {
        Vertices Vertices { get; }
        Point Centroid { get; }
        float Area { get; }
        bool Contains(Point point);
    }
}