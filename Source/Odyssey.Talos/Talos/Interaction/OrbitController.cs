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
using Odyssey.Talos;
using Odyssey.Talos.Components;
using SharpDX;

#endregion

namespace Odyssey.Talos.Interaction
{
    public class OrbitController : PointerControllerBase
    {
        private TargetComponent cTarget;

        private float currentPitch;
        private float currentYaw;
        private bool isDragging;
        private Vector2 pCurrent;
        private Vector2 pPrev;

        public OrbitController(IServiceRegistry services)
            : base(services)
        {
            CameraOffset = 10.0f;
            RotationSpeed = 0.25f;
        }

        public float CameraOffset { get; set; }

        public float RotationSpeed { get; set; }

        public override void BindToEntity(Entity source)
        {
            base.BindToEntity(source);
            cTarget = source.GetComponent<TargetComponent>();
            
            // Initialise values based on current orientation
            Quaternion q = CRotation.Orientation;
            currentYaw = (float) Math.Asin(2*q.X*q.Y + 2*q.Z*q.W);
            currentPitch = -(float) Math.Atan2(2*q.X*q.W - 2*q.Y*q.Z, 1 - 2*q.X*q.X - 2*q.Z*q.Z);
        }

        public override void Update(ITimeService time)
        {
            base.Update(time);

            Quaternion q = Quaternion.RotationAxis(Vector3.UnitX, currentPitch)*Quaternion.RotationAxis(Vector3.UnitY, currentYaw);
            Vector3 vForward = Vector3.Transform(Vector3.UnitZ, q);
            CPosition.Position = cTarget.Location - (vForward*CameraOffset);
            CRotation.Orientation = Quaternion.LookAtRH(CPosition.Position, cTarget.Location, Vector3.Up);
        }

        protected override void PointerPressed(PointerPoint point, ITimeService time)
        {
            if (point.IsLeftButtonPressed)
            {
                pPrev = point.Position;
                isDragging = true;
            }
        }

        protected override void PointerMoved(PointerPoint point, ITimeService time)
        {
            if (!isDragging)
                return;
            pCurrent = point.Position;
            Vector2 delta = (pCurrent - pPrev);
            currentYaw -= MathUtil.DegreesToRadians(RotationSpeed*delta.X);
            currentPitch += MathUtil.DegreesToRadians(RotationSpeed*delta.Y);

            pPrev = pCurrent;
        }

        protected override void PointerReleased(PointerPoint point, ITimeService time)
        {
            isDragging = false;
        }
    }
}