using System.Linq;
using Odyssey.Engine;
using Odyssey.Talos.Components;
using Odyssey.Talos.Messages;
using Odyssey.Talos.Nodes;

namespace Odyssey.Talos.Systems
{
    public sealed class PointLightSystem : LightSystem
    {
        public PointLightSystem()
            : base(Selector.All(typeof(PositionComponent), typeof(PointLightComponent), typeof(UpdateComponent)))
        {
        }

        void SetupEntity(Entity entity)
        {
            PointLightNode nPointLight = new PointLightNode(entity);
            LightNodes.Add(nPointLight.Id, nPointLight);
            Messenger.Send(new LightMessage(entity, nPointLight, ChangeType.Removed));
        }

        public override void BeforeUpdate()
        {
            // TODO improve Light System
            // Entity change
            while (MessageQueue.HasItems<EntityChangeMessage>())
            {
                EntityChangeMessage mEntity = MessageQueue.Dequeue<EntityChangeMessage>();
                if (mEntity.Action == ChangeType.Added)
                    SetupEntity(mEntity.Source);
                else if (mEntity.Action == ChangeType.Removed)
                    RemoveEntity(mEntity.Source);
            }
        }


        public override void Process(ITimeService time)
        {
            foreach (Entity entity in Entities)
            {
                if (!entity.IsEnabled)
                    continue;
                var cUpdate = entity.GetComponent<UpdateComponent>(Update.KeyPart);
                if (!cUpdate.RequiresUpdate)
                    continue;

                LightNode lightNode = (from kvp in LightNodes
                    where kvp.Value.EntityId == entity.Id
                    select kvp.Value).First();

                lightNode.Update();
            }
        }
    }
}
