using System.Collections.Generic;
using System.Linq;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.Graphics.Effects;
using Odyssey.Graphics.Models;
using Odyssey.Graphics.Organization;
using Odyssey.Graphics.Organization.Commands;
using Odyssey.Graphics.Shaders;
using Odyssey.Organization;
using Odyssey.Organization.Commands;
using Odyssey.Talos.Components;
using Odyssey.Talos.Messages;
using Odyssey.Utilities.Logging;
using SharpYaml.Serialization;

namespace Odyssey.Talos.Systems
{
    public class OptimizationSystem : UpdateableSystemBase
    {
        private readonly RenderMapper renderMapper;
        readonly ComponentType tShader;
        readonly ComponentType tModel;

        public OptimizationSystem() : base(Selector.All(typeof (ModelComponent), typeof(ShaderComponent)))
        {
            tShader = ComponentTypeManager.GetType<ShaderComponent>();
            tModel = ComponentTypeManager.GetType<ModelComponent>();
            renderMapper = new RenderMapper();
        }

        public override void BeforeUpdate()
        {
            renderMapper.Clear();
        }

        public override void Process(ITimeService time)
        {
            foreach (Entity entity in Entities.Where(e => e.IsEnabled))
            {
                var cModel = entity.GetComponent<ModelComponent>(tModel.KeyPart);
                var cShader = entity.GetComponent<ShaderComponent>(tShader.KeyPart);
                
                renderMapper.AddEntity(cShader.Technique, cModel.Model, entity);
            }
        }

        public override void AfterUpdate()
        {
            renderMapper.Group();
            CreateCommands();
        }

        void CreateCommands()
        {
            LinkedList<Command> commands = new LinkedList<Command>();
            foreach (var renderable in renderMapper)
            {
                Technique technique = renderable.Technique;
                Model model = renderable.Model;
                RenderCommand renderCommand;
                if (technique.Mapping.Key.Supports(VertexShaderFlags.InstanceWorld))
                {
                    renderCommand = new InstancingRenderCommand(Services, technique, model, renderable.Entities);
                }
                else
                {
                    renderCommand = new TechniqueRenderCommand(Services, technique, model, renderable.Entities);
                }
                commands.AddLast(renderCommand);
            }


            StateViewer sv = new StateViewer(Services, commands);
            LinkedList<Command> resultCommands = sv.Analyze();

            if (resultCommands == null)
                LogEvent.Engine.Error("Nothing to render!");
            else
            {
                Messenger.Send(new OptimizationCompleteMessage(resultCommands));
            }
            IsEnabled = false;

        }
    }
}
