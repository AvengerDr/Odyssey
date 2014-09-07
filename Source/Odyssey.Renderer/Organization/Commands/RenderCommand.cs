using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Odyssey.Graphics.Models;
using Odyssey.Graphics.Organization;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos;
using SharpDX;

namespace Odyssey.Organization.Commands
{
    public abstract class RenderCommand : EngineCommand, IRenderCommand
    {
        private readonly Technique technique;
        private readonly List<IEntity> entities;
        
        public Model Model { get; protected set; }
        protected int EntityCount { get { return entities.Count(); }}
        

        public IEnumerable<IEntity> Entities { get { return entities; } }

        public Technique Technique { get { return technique; } }

        protected RenderCommand(IServiceRegistry services, Technique effect, IEnumerable<IEntity> entities)
            : base(services, CommandType.Render)
        {
            Contract.Requires<ArgumentNullException>(effect != null, "effect");
            Contract.Requires<ArgumentNullException>(entities != null, "entities");
            Contract.Requires<ArgumentException>(entities.Any(), "[entities] sequence is empty");
            this.technique = ToDispose(effect);
            this.entities = new List<IEntity>(entities);
        }

        public override void Execute()
        {
            PreRender();
            Render();
            PostRender();
        }

#if DEBUG
        //public bool CheckPreconditions()
        //{
            //bool test = true;
            //    foreach (var resource in Material.Resources)
            //    {
            //        if (resource == null)
            //        {
            //            LogEvent.Engine.Warning("Null resource returned from {0}.", Material.Name);
            //            test = false;
            //            continue;
            //        }

            //        test = ((IValidable)resource).Validate();
            //    }

            //Items.SelectMany(item => item.Meshes).All(mesh => mesh.CheckFormat(Material.ItemsDescription.VertexFormat));
            //return test;
        //}
#endif

        public abstract void PreRender();
        public abstract void PostRender();
        public abstract void Render();

    }
}