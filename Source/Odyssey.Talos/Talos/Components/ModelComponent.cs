using Odyssey.Graphics.Models;
using System;
using System.Diagnostics.Contracts;
using Odyssey.Talos.Messages;

namespace Odyssey.Talos.Components
{
    [OptionalComponent(typeof(ShaderComponent), typeof(PostProcessComponent))]
    [RequiredComponent(typeof(PositionComponent))]
    [RequiredComponent(typeof(TransformComponent))]
    public sealed class ModelComponent : ContentComponent
    {
        public Model Model { get; private set; }
        public override bool IsInited { get { return Model != null; } }

        
        public ModelComponent()
            : base(ComponentTypeManager.GetType<ModelComponent>())
        {
        }

        public override void Initialize()
        {
            if (IsProcedurallyGenerated)
                return;
            Model = Content.Load<Model>(AssetName);
        }

        protected override void OnReceiveMessage(MessageEventArgs e)
        {
            base.OnReceiveMessage(e);

            var mContent = e.Message as ContentMessage<Model>;
            if (mContent == null)
                throw new InvalidOperationException(string.Format("Message of type '{0}' is not supported", e.Message.GetType()));
            Model = mContent.Content;
        }
    }
}
