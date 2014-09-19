using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Talos.Components;
using Odyssey.Talos.Messages;

namespace Odyssey.Talos.Systems
{
    public abstract class RunOnceSystem : UpdateableSystemBase
    {
        protected RunOnceSystem(Selector selector) : base(selector) { }

        public override void Start()
        {
            Messenger.Register<EntityChangeMessage>(this);
        }

        public override void Stop()
        {
            Messenger.Unregister<EntityChangeMessage>(this);
        }

        public override void BeforeUpdate()
        {
            while (MessageQueue.HasItems<EntityChangeMessage>())
            {
                var mEntity = MessageQueue.Dequeue<EntityChangeMessage>();
                if (mEntity.Action == UpdateType.Add)
                {
                    RegisterEntity(mEntity.Source);
                }
            }

            if (HasEntities)
                IsEnabled = true;
        }

        public override void AfterUpdate()
        {
            var entitiesCopy = new List<Entity>(Entities);

            foreach (Entity entity in entitiesCopy)
            {
                Scene.SystemMap.UnregisterEntityFromSystem(entity, this);
            }

            IsEnabled = false;
        }
    }
}
