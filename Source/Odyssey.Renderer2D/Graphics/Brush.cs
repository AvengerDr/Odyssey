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

using System;
using System.Linq;
using Odyssey.Engine;
using SharpDX;
using Odyssey.Animations;

#endregion Using Directives

namespace Odyssey.Graphics
{
    public abstract class Brush : Direct2DResource
    {
        protected new readonly SharpDX.Direct2D1.Brush Resource;

        protected Brush(string name, Direct2DDevice device, SharpDX.Direct2D1.Brush brush)
            : base(name, device)
        {
            Resource = brush;
        }

        public override void Initialize()
        {
            Initialize(ToDispose(Resource));
        }

        public Matrix3x2 Transform
        {
            get { return Resource.Transform; }
            set { Resource.Transform = value; }
        }

        /// <summary>
        /// <see cref="SharpDX.Direct2D1.Brush"/> casting operator.
        /// </summary>
        /// <param name="from">From the Texture1D.</param>
        public static implicit operator SharpDX.Direct2D1.Brush(Brush from)
        {
            return from == null ? null : from.Resource ?? null;
        }

        internal static Brush FromColorResource(Direct2DDevice device, ColorResource colorResource)
        {
            switch (colorResource.Type)
            {
                case GradientType.SolidColor:
                    return SolidColorBrush.New(colorResource.Name, device, (SolidColor)colorResource);
                case GradientType.Linear:
                    return LinearGradientBrush.New(colorResource.Name, device, (LinearGradient)colorResource);
                case GradientType.Radial:
                    return RadialGradientBrush.New(colorResource.Name, device, (RadialGradient) colorResource);
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Brush type '{0}' is not valid", colorResource.Type));
            }
        }


    }
}