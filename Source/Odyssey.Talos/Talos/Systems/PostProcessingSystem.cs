using System;
using System.Collections.Generic;
using System.Linq;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.Graphics.Organization.Commands;
using Odyssey.Graphics.PostProcessing;
using Odyssey.Graphics.Shaders;
using Odyssey.Organization.Commands;
using Odyssey.Talos.Components;
using Odyssey.Talos.Messages;

namespace Odyssey.Talos.Systems
{
    public class PostProcessingSystem : UpdateableSystemBase, IRenderableSystem
    {
        private readonly CommandManager commandManager;

        public PostProcessingSystem() : base(Selector.All(typeof(PostProcessComponent)))
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
            return true;
        }

        public void Render(ITimeService service)
        {
            foreach (Entity entity in Entities)
            {
                commandManager.Run();
            }
        }

        public override void Unload()
        {
            commandManager.Unload();
        }

        public override void Process(ITimeService time)
        {
            if (!MessageQueue.HasItems<OptimizationCompleteMessage>()) return;

            var mOptimization = MessageQueue.Dequeue<OptimizationCompleteMessage>();
            if (!mOptimization.Commands.Any())
                return;

            if (!commandManager.IsEmpty)
                commandManager.Clear();

            List<Command> newCommands = new List<Command>();
            foreach (Entity entity in Entities)
            {
                var cPostProcess = entity.GetComponent<PostProcessComponent>();
                var cModel = entity.GetComponent<ModelComponent>();

                var techniques = cPostProcess.Techniques.ToDictionary(t => t.Name, t => t);
                for (int i=0; i< cPostProcess.Actions.Count; i++)
                {
                    PostProcessAction ppAction = cPostProcess.Actions[i];

                    if (ppAction.AssetName == Param.Odyssey)
                    {
                        if (ppAction.Technique == Param.EngineActions.RenderSceneToTexture)
                        {
                            string tagFilter = cPostProcess.TagFilter;
                            var filteredCommands = FilterCommands(mOptimization.Commands, tagFilter);
                            if (filteredCommands == null)
                                continue;
                            var cRender2Texture = new RenderSceneToTextureCommand(Services, filteredCommands);
                            newCommands.Add(cRender2Texture);
                        }
                    }
                    else
                    {
                        Technique effect = techniques[string.Format("{0}.{1}", ppAction.AssetName, ppAction.Technique)];
                        newCommands.Add(new PostProcessCommand(Services, effect, cModel.Model.Meshes[0],
                            entity, ppAction.TextureDescription, ppAction.Output) {Name = ppAction.Technique});
                    }
                }
                
                StateViewer sv = new StateViewer(Services, newCommands);
                commandManager.AddLast(sv.Analyze());
                commandManager.Initialize();

                if (MessageQueue.HasItems<OptimizationCompleteMessage>())
                    throw new InvalidOperationException(string.Format("Multiple {0}s found", typeof(OptimizationCompleteMessage).Name));
                var postProcessCommands = commandManager.OfType<IPostProcessCommand>().ToArray();

                for (int i = 1; i < postProcessCommands.Length; i++)
                {
                    var ppAction = cPostProcess.Actions[i];
                    postProcessCommands[i].SetInputs(ppAction.InputIndices.Select(idx => postProcessCommands[idx].Output));
                }
            }

            
        }


        IEnumerable<Command> FilterCommands(IEnumerable<Command> commands, string tagFilter)
        {
            List<Command> filteredCommands =
                (from cRender in commands.OfType<RenderCommand>()
                    let filteredEntities = from e in cRender.Entities 
                                           where e.ContainsTag(tagFilter)
                                           select e
                    where filteredEntities.Any()
                    let tRenderCommand = cRender.GetType()
                    select (RenderCommand) Activator.CreateInstance(tRenderCommand,
                                new object[] { Services, cRender.Technique, cRender.Model, filteredEntities }))
                                .Cast<Command>().ToList();

            StateViewer sv = new StateViewer(Services, filteredCommands);
            return sv.Analyze(); 
        }
        
    }
}
