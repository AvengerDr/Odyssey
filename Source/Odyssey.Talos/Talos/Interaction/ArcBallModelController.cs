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

#endregion

#region Using Directives

using System;
using Odyssey.Engine;
using Odyssey.Interaction;
using SharpDX;

#endregion

namespace Odyssey.Talos.Interaction
{
    public class ArcBallModelController : PointerControllerBase
    {
        private float arcBallRadius;
        private bool isDragging;
        private Vector2 lastPointerPosition;
        private Quaternion qCurrent;
        private Quaternion qStart;
        private Vector3 sphereStart;

        public ArcBallModelController(IServiceRegistry services) : base(services)
        {
            arcBallRadius = 0.85f;
        }

        /// <summary>
        /// ArcBall radius multiplier.
        /// </summary>
        public float ArcBallRadius
        {
            get { return arcBallRadius; }
            set { arcBallRadius = value; }
        }

        protected override void PointerPressed(PointerPoint point, ITimeService time)
        {
            lastPointerPosition = point.Position;
            if (point.IsLeftButtonPressed)
            {
                isDragging = true;
                sphereStart = MapToArcBall(lastPointerPosition, ScreenSize, arcBallRadius);
                qStart = CRotation.Orientation;
            }
        }

        protected override void PointerMoved(PointerPoint point, ITimeService time)
        {
            if (isDragging)
            {
                Vector2 currentPoint = point.Position;
                UpdateEntity(currentPoint);
            }

            lastPointerPosition = point.Position;
        }

        protected override void PointerReleased(PointerPoint point, ITimeService time)
        {
            if (point.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
            {
                isDragging = false;
                qStart = CRotation.Orientation;
                qCurrent = Quaternion.Identity;
            }
        }

        private static Vector3 MapToArcBall(Vector2 point, Vector2 screenSize, float arcBallRadius)
        {
            //// Adjust point coords and scale down to range of [-1 ... 1]
            float x = -(point.X - screenSize.X/2)/(arcBallRadius*screenSize.X/2);
            float y = (point.Y - screenSize.Y/2)/(arcBallRadius*screenSize.Y/2);

            float z = 0.0f;
            float mag = x*x + y*y;

            if (mag > 1.0f)
            {
                float scale = 1.0f/(float) Math.Sqrt(mag);
                x *= scale;
                y *= scale;
            }
            else
                z = (float) Math.Sqrt(1.0f - mag);

            // Return vector
            return new Vector3(x, y, z);
        }

        protected virtual void UpdateEntity(Vector2 current)
        {
            if (current.X < 0 || current.X > ScreenSize.X)
                return;

            if (current.Y < 0 || current.Y > ScreenSize.Y)
                return;

            Vector3 currentPoint = MapToArcBall(current, ScreenSize, arcBallRadius);

            // Computes the quaternion necessary to rotate from the previous 
            // ArcBall point to the current one
            Vector3 axis = Vector3.Cross(currentPoint, sphereStart);
            float dot = Vector3.Dot(sphereStart, currentPoint);
            qCurrent = new Quaternion(axis, dot);
            // Rotation relative to local Frame
            Quaternion rot = Quaternion.Normalize(qCurrent*qStart);
            CRotation.Orientation = rot;
        }
    }
}