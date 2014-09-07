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

using Odyssey.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

#endregion Using Directives

#if !WP8

namespace Odyssey.Graphics.Drawing
{
    public class GeometryGroup : Geometry
    {
        protected new readonly SharpDX.Direct2D1.GeometryGroup Resource;

        protected GeometryGroup(string name, Direct2DDevice device, FillMode fillMode, SharpDX.Direct2D1.Geometry[] geometries)
            : base(name, device)
        {
            Contract.Requires<ArgumentNullException>(geometries!=null, "geometries");
            Contract.Requires<InvalidOperationException>(geometries.Length>0, "At least one geometry needed");
            Resource = new SharpDX.Direct2D1.GeometryGroup(Device, GetD2DFlag(fillMode), geometries.ToArray());
        }

        /// <summary>
        /// <see cref="SharpDX.Direct2D1.GeometryGroup"/> casting operator.
        /// </summary>
        /// <param name="from">From the Texture1D.</param>
        public static implicit operator SharpDX.Direct2D1.GeometryGroup(GeometryGroup from)
        {
            // Don't bother with multithreading here
            return from == null ? null : from.Resource ?? null;
        }

        public static GeometryGroup New(string name, Direct2DDevice device, FillMode fillMode, IEnumerable<Geometry> geometries)
        {
            var nativeGeometries = geometries.Select(geometry => (SharpDX.Direct2D1.Geometry) geometry).ToArray();
            return new GeometryGroup(name, device, fillMode, nativeGeometries);
        }

        private static SharpDX.Direct2D1.FillMode GetD2DFlag(FillMode fillMode)
        {
            switch (fillMode)
            {
                case FillMode.Alternate:
                    return SharpDX.Direct2D1.FillMode.Alternate;

                case FillMode.Winding:
                    return SharpDX.Direct2D1.FillMode.Winding;

                default:
                    throw new ArgumentOutOfRangeException("fillMode");
            }
        }
    }
}

#endif