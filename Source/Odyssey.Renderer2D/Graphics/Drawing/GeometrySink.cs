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

using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion Using Directives

#if !WP8

namespace Odyssey.Graphics.Drawing
{
    public class GeometrySink : Component
    {
        private readonly SharpDX.Direct2D1.GeometrySink geometrySink;
        private bool isFigureOpened;

        internal GeometrySink(SharpDX.Direct2D1.GeometrySink sink)
        {
            geometrySink = ToDispose(sink);
        }

        public void AddLine(Vector2 point)
        {
            geometrySink.AddLine(point);
        }

        public void AddLines(IEnumerable<Vector2> points)
        {
            geometrySink.AddLines(points.ToArray());
        }

        public void BeginFigure(Vector2 point, FigureBegin figureBegin)
        {
            isFigureOpened = true;
            geometrySink.BeginFigure(point, GetD2DFigureFlag(figureBegin));
        }

        public void Close()
        {
            geometrySink.Close();
            geometrySink.Dispose();
        }

        public void EndFigure(FigureEnd figureEnd)
        {
            if (!isFigureOpened)
                throw new InvalidOperationException("Figure was not opened.");
            geometrySink.EndFigure(GetD2DFigureFlag(figureEnd));
            isFigureOpened = false;
        }

        private static SharpDX.Direct2D1.FigureBegin GetD2DFigureFlag(FigureBegin figureBegin)
        {
            switch (figureBegin)
            {
                case FigureBegin.Filled:
                    return SharpDX.Direct2D1.FigureBegin.Filled;

                case FigureBegin.Hollow:
                    return SharpDX.Direct2D1.FigureBegin.Hollow;

                default:
                    throw new ArgumentOutOfRangeException("figureBegin");
            }
        }

        private static SharpDX.Direct2D1.FigureEnd GetD2DFigureFlag(FigureEnd figureEnd)
        {
            switch (figureEnd)
            {
                case FigureEnd.Open:
                    return SharpDX.Direct2D1.FigureEnd.Open;

                case FigureEnd.Closed:
                    return SharpDX.Direct2D1.FigureEnd.Closed;

                default:
                    throw new ArgumentOutOfRangeException("figureEnd");
            }
        }
    }
}

#endif