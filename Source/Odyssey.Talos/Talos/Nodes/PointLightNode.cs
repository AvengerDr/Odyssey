using Odyssey.Geometry;
using Odyssey.Talos.Components;
using System;
using SharpDX;

namespace Odyssey.Talos.Nodes
{
    public class PointLightNode : LightNode
    {
        public PointLightNode(Entity entity) : base(entity)
        {
            LightComponent = entity.GetComponent<PointLightComponent>();
        }

        public override void Update()
        {
            WorldPosition = Vector3.Transform(PositionComponent.Position, TransformComponent.World).ToVector3();
            //WorldPosition = PositionComponent.Position;
        }


    }
}
