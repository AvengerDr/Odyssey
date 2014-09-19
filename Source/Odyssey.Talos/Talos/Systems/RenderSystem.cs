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
            Messenger.Register<CommandUpdateMessage>(this);
        }

        public override void Stop()
        {
            Messenger.Unregister<CommandUpdateMessage>(this);
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
            if (!MessageQueue.HasItems<CommandUpdateMessage>()) return;

            var mUpdate = MessageQueue.Dequeue<CommandUpdateMessage>();

            switch (mUpdate.UpdateType)
            {
                case UpdateType.Add:
                    commandManager.AddLast(mUpdate.Commands);
                    commandManager.Initialize();
                    break;
            }
        }
    }
}
