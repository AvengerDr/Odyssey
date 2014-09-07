using Odyssey.Engine;
using Odyssey.Talos.Components;
using Odyssey.Talos.Messages;
using Odyssey.UserInterface.Controls;

namespace Odyssey.Talos.Systems
{
    public class UserInterfaceSystem : UpdateableSystemBase, IRenderableSystem
    {
        private readonly ComponentType tUserInterface;

        protected Overlay Overlay { get; private set; }

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

        public bool BeginRender()
        {
            return true;
        }

        public void Render(ITimeService time)
        {
            foreach (Entity entity in Entities)
            {
                var cUserInterface = entity.GetComponent<UserInterfaceComponent>(tUserInterface.KeyPart);
                cUserInterface.Overlay.Display();
            }
        }

        public override void BeforeUpdate()
        {
            while (MessageQueue.HasItems<EntityChangeMessage>())
            {
                var mEntityChange = MessageQueue.Dequeue<EntityChangeMessage>();
                if (mEntityChange.Action == ChangeType.Added)
                {
                    var cUserInterface = mEntityChange.Source.GetComponent<UserInterfaceComponent>(tUserInterface.KeyPart);
                    cUserInterface.Initialize();
                    Overlay = cUserInterface.Overlay;
                }
            }
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
            if (Overlay!=null)
                Overlay.Dispose();
        }
    }
}