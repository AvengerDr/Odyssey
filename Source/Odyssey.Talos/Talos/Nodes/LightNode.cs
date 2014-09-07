using Odyssey.Talos.Components;
using SharpDX;

namespace Odyssey.Talos.Nodes
{
    public abstract class LightNode
    {
        private readonly PositionComponent cPosition;

        private readonly TransformComponent cTransform;

        private readonly UpdateComponent cUpdate;

        private readonly IEntity entity;

        public long EntityId { get { return entity.Id; } }
        public int Id { get { return LightComponent.LightId; } }

        public PositionComponent PositionComponent { get { return cPosition; } }

        public TransformComponent TransformComponent { get { return cTransform; } }

        public LightComponent LightComponent { get; protected set; }

        public UpdateComponent UpdateComponent { get { return cUpdate; } }

        internal Vector3 WorldPosition { get; set; }

        protected LightNode(Entity entity)
        {
            this.entity = entity;
            cPosition = entity.GetComponent<PositionComponent>();
            cUpdate = entity.GetComponent<UpdateComponent>();
            cTransform = entity.GetComponent<TransformComponent>();
        }

        public abstract void Update();

       
    }
}