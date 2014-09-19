using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odyssey.Engine;
using Odyssey.Talos.Components;
using Odyssey.Talos.Messages;

namespace Odyssey.Talos.Systems
{
    public class ControllerSystem : UpdateableSystemBase
    {
        public ControllerSystem() : base(Selector.All(typeof(ControllerComponent)))
        {
        }

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
            // Entity change
            while (MessageQueue.HasItems<EntityChangeMessage>())
            {
                EntityChangeMessage mEntity = MessageQueue.Dequeue<EntityChangeMessage>();
                var entity = mEntity.Source;
                var cController = entity.GetComponent<ControllerComponent>();

                if (mEntity.Action == UpdateType.Add)
                    cController.Controller.BindToEntity(entity);
            }
        }

        public override void Process(ITimeService time)
        {
            foreach (var entity in Entities)
            {
                var cController = entity.GetComponent<ControllerComponent>();
                cController.Controller.Update(time);
            }
        }
    }
}
