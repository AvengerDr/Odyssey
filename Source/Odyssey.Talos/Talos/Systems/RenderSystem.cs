using System;
using Odyssey.Engine;
using Odyssey.Graphics.Organization.Commands;
using Odyssey.Organization.Commands;
using Odyssey.Talos.Components;
using Odyssey.Talos.Messages;
using SharpDX;
using SharpYaml.Serialization;

namespace Odyssey.Talos.Systems
{
    [YamlTag("RenderSystem")]
    public class RenderSystem : UpdateableSystemBase, IRenderableSystem
    {
        private readonly CommandManager commandManager;

        public RenderSystem() : base(Selector.All(typeof (ModelComponent), typeof (ShaderComponent)))
        {
            commandManager = new CommandManager();
        }

        public override void Start()
        {
            Messenger.Register<OptimizationCompleteMessage>(this);
        }

        public override void Stop()
        {
            Messenger.Unregister<OptimizationCompleteMessage>(this);
        }

        public bool BeginRender()
        {
            var deviceService = Scene.Services.GetService<IOdysseyDeviceService>();
            deviceService.DirectXDevice.Clear(Color.Black);
            return true;
        }

        public void Render(ITimeService time)
        {
            foreach (Command command in commandManager)
                command.Execute();
        }

        public override void Unload()
        {
            commandManager.Unload();
        }

        public override void Process(ITimeService time)
        {
            if (!MessageQueue.HasItems<OptimizationCompleteMessage>()) return;

            var mOptimization = MessageQueue.Dequeue<OptimizationCompleteMessage>();
            if (!commandManager.IsEmpty)
                commandManager.Clear();

            commandManager.AddLast(mOptimization.Commands);
            commandManager.Initialize();

            if (MessageQueue.HasItems<OptimizationCompleteMessage>())
                throw new InvalidOperationException(string.Format("Multiple {0}s found", typeof(OptimizationCompleteMessage).Name));
        }
    }
}
