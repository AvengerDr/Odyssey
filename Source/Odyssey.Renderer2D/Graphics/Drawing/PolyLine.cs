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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Odyssey.Engine;
using SharpDX;

#endregion Using Directives

namespace Odyssey.Graphics.Drawing
{
    public class PolyLine : PathGeometry
    {
        internal PolyLine(string name, Direct2DDevice device)
            : base(name, device)
        {
            Initialize(Resource);
        }

        public static PolyLine New(string name, Direct2DDevice device, IEnumerable<Vector2> points, FigureBegin figureBegin,
            FigureEnd figureEnd)
        {
            Contract.Requires<ArgumentNullException>(points != null);
            var array = points as Vector2[] ?? points.ToArray();
            if (array.Length < 2)
                throw new ArgumentException("Figure needs at least two points");
            
            PolyLine polyLine = new PolyLine(name, device);
            var sink = polyLine.DefineFigure();
            sink.BeginFigure(array[0], figureBegin);
            sink.AddLines(array.Skip(1));
            sink.EndFigure(figureEnd);
            sink.Close();
            return polyLine;
        }
    }
}