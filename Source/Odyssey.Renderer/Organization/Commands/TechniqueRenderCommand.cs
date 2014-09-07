using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.Graphics.Effects;
using Odyssey.Graphics.Models;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos;
using Odyssey.Utilities.Logging;
using SharpDX;

namespace Odyssey.Organization.Commands
{
    [DebuggerDisplay("{Type}[{Technique.Name}]: {entities.Count} items [{Model.Name}]")]
    public class TechniqueRenderCommand : RenderCommand, ITechniqueRenderCommand
    {
        public TechniqueRenderCommand(IServiceRegistry services, Technique technique, Model model, IEnumerable<IEntity> entities)
            : base(services, technique, entities)
        {
            Contract.Requires<ArgumentNullException>(model != null, "model");

            Model = model;
            Name = string.Format("{0}[{1}]", Type, Technique.Name);
        }

        public override void Initialize()
        {
            //Effect.AssembleBuffers(directX);
#if DEBUG
            CheckPreconditions();
#endif
        }

        public override void PostRender()
        {
            DirectXDevice device = DeviceService.DirectXDevice;
            if (Technique[ShaderType.Pixel].HasTextures())
                foreach (TextureMapping tm in Technique[ShaderType.Pixel].SelectTextures(UpdateType.SceneStatic))
                {
                    device.SetPixelShaderShaderResourceView(tm.Description.Index, null);
                    device.SetPixelShaderSampler(tm.Description.SamplerIndex, null);
                }
        }

        public override void PreRender()
        {
            DirectXDevice device = DeviceService.DirectXDevice;
            device.SetCurrentEffect(Technique);

            device.InputAssembler.InputLayout = Technique.InputLayout;

            foreach (TextureMapping tm in Technique[ShaderType.Pixel].SelectTextures(UpdateType.SceneStatic))
            {
                // TODO Update SamplerState assignment
                device.SetPixelShaderSampler(tm.Description.SamplerIndex, device.SamplerStates.Default);
                device.SetPixelShaderShaderResourceView(tm.Description.Index, tm.Texture.ShaderResourceView[ViewType.Full, 0, 0]);
            }

            foreach (Shader shader in Technique)
                shader.Apply(Technique.Name, UpdateType.SceneStatic);
        }

        public override void Render()
        {
            DirectXDevice device = DeviceService.DirectXDevice;
            
            foreach (Shader shader in Technique)
                shader.Apply(Technique.Name, UpdateType.SceneFrame);

            foreach (IEntity entity in Entities)
            {
                if (!entity.IsEnabled)
                    continue;

                foreach (Shader shader in Technique)
                {
                    shader.Apply(Technique.Name, entity.Id, UpdateType.InstanceStatic);
                    shader.Apply(Technique.Name, entity.Id, UpdateType.InstanceFrame);
                }

                foreach (ModelMesh mesh in Model.Meshes)
                {
                    mesh.Draw(device);
                }
            }
        }

        private void CheckPreconditions()
        {
            bool result = true;
            foreach (TextureMapping tm in Technique[ShaderType.Pixel].SelectTextures().Where(tm => tm.Texture == null))
            {
                LogEvent.Engine.Error("{0} is null [{1}.{2}].", tm.Description.Key, Technique[ShaderType.Pixel].Name,
                    tm.Description.Texture);
                result = false;
            }
            if (!result)
                throw new InvalidOperationException(string.Format("[{0}]: preconditions failed.", Name));
        }

    }
}