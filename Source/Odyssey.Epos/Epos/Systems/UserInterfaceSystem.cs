﻿using Odyssey.Engine;
using Odyssey.Epos.Components;
using Odyssey.Epos.Messages;
using Odyssey.Organization.Commands;

namespace Odyssey.Epos.Systems
{
    public class UserInterfaceSystem : UpdateableSystemBase
    {
        private readonly ComponentType tUserInterface;

        public UserInterfaceSystem()
            : base(Selector.All(typeof(UserInterfaceComponent)))
        {
            tUserInterface = ComponentTypeManager.GetType<UserInterfaceComponent>();
        }

        public override void Start()
        {
            Messenger.Register<EntityChangeMessage>(this);
        }

        public override void Stop()
        {
            Messenger.Unregister<EntityChangeMessage>(this);
        }

        public override bool BeforeUpdate()
        {
            while (MessageQueue.HasItems<EntityChangeMessage>())
            {
                var mEntityChange = MessageQueue.Dequeue<EntityChangeMessage>();
                if (mEntityChange.Action == UpdateType.Add)
                {
                    var cUserInterface = mEntityChange.Source.GetComponent<UserInterfaceComponent>(tUserInterface.KeyPart);
                    cUserInterface.Initialize();
                    Messenger.SendToSystem<RenderSystem, CommandUpdateMessage>(new CommandUpdateMessage(new UserInterfaceRenderCommand(Services, cUserInterface.Overlay)));
                }
            }
            return base.BeforeUpdate();
        }

        public override void Process(ITimeService time)
        {
            foreach (Entity entity in Entities)
            {
                var cUserInterface = entity.GetComponent<UserInterfaceComponent>(tUserInterface.KeyPart);
                cUserInterface.UserInterfaceState.Update();
                cUserInterface.Overlay.Update(time);
            }
        }

        public override void Unload()
        {
            foreach (Entity entity in Entities)
            {
                var cUserInterface = entity.GetComponent<UserInterfaceComponent>(tUserInterface.KeyPart);
                cUserInterface.Overlay.Dispose();
            }
        }
    }
}