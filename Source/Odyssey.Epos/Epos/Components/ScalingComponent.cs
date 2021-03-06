﻿using System.Diagnostics;
using SharpDX.Mathematics;

namespace Odyssey.Epos.Components
{
    [DebuggerDisplay("{Scale}: ({Scaling})")]
    public class ScalingComponent : Component
    {
        public Vector3 Scale { get; set; }

        public ScalingComponent() : base(ComponentTypeManager.GetType<ScalingComponent>())
        {
            Scale = new Vector3(1, 1, 1);
        }
    }
}
