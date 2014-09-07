using Odyssey.Talos.Components;
using Odyssey.Talos.Messages;

namespace Odyssey.Talos.Systems
{
    public class AnimationSystem : UpdateableSystemBase
    {
        public AnimationSystem() : base(Selector.All(typeof(AnimationComponent)))
        { }

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
                var cAnimation = entity.GetComponent<AnimationComponent>();

                cAnimation.Initialize();
            }
        }

        public override void Process(Engine.ITimeService time)
        {
            foreach (Entity entity in Entities)
            {
                var cAnimation = entity.GetComponent<AnimationComponent>();
                cAnimation.Controller.Update(time);
            }
        }
    }
}
