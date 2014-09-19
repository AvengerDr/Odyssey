using Odyssey.Engine;
using Odyssey.Graphics.Effects;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos.Components;
using Odyssey.Talos.Initializers;
using Odyssey.Talos.Messages;
using System;
using System.Linq;
using UpdateType = Odyssey.Graphics.Effects.UpdateType;

namespace Odyssey.Talos.Systems
{
    public class InitializationSystem : UpdateableSystemBase
    {
        public InitializationSystem()
            : base(Selector.One(typeof(ShaderComponent), typeof(PostProcessComponent)))
        {
        }

        public override void BeforeUpdate()
        {
            // Before each frame we check if a new shader has been added to the scene.
            // If so, we need to initialize it.
            while (MessageQueue.HasItems<ContentMessage<Technique>>())
            {
                var mTechnique = MessageQueue.Dequeue<ContentMessage<Technique>>();
                SetupEntity(mTechnique.Content);
            }
        }

        public override void Process(ITimeService time)
        {
            var data = (from entity in Entities
                        let techniqueComponents = entity.Components.OfType<ITechniqueComponent>()
                        from techniqueRange in techniqueComponents
                        from technique in techniqueRange.Techniques
                        select technique).Distinct();
            // Update each per frame constant buffer
            foreach (Technique technique in data)
            {
                technique.UpdateBuffers(UpdateType.InstanceStatic);
                technique.UpdateBuffers(UpdateType.SceneFrame);
                technique.UpdateBuffers(UpdateType.InstanceFrame);
            }
        }

        public override void Start()
        {
            Messenger.Register<ContentMessage<Technique>>(this);
        }

        public override void Stop()
        {
            Messenger.Unregister<ContentMessage<Technique>>(this);
        }

        private void SetupEntity(Technique technique)
        {
            Technique effect = technique;
            var shaderInitializer = new ShaderInitializer(Services, technique);
            shaderInitializer.Initialize();

            if (!technique.Mapping.Validate())
                throw new InvalidOperationException(string.Format("[{0}] was not properly initialized.", technique.Name));

            effect.AssembleBuffers();
        }
    }
}