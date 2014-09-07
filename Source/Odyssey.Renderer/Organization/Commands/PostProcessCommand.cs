using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.Graphics.Effects;
using Odyssey.Graphics.Models;
using Odyssey.Graphics.PostProcessing;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos;
using SharpDX;
using SharpDX.Direct3D11;

namespace Odyssey.Organization.Commands
{
    [DebuggerDisplay("{Type}[{technique.Name}]")]
    public class PostProcessCommand : RenderToTextureCommandBase, ITechniqueRenderCommand
    {
        private readonly Technique technique;
        private readonly IEntity entity;
        private readonly ModelMesh quad;
        public Technique Technique { get { return technique; } }

        public PostProcessCommand(IServiceRegistry services, Technique technique, ModelMesh quad, IEntity entity, Texture2DDescription textureDescription = default (Texture2DDescription), TargetOutput output = TargetOutput.NewRenderTarget) 
            : base(services, textureDescription, output)
        {
            Contract.Requires<ArgumentNullException>(technique != null, "technique");
            Contract.Requires<ArgumentNullException>(quad!= null, "quad");
            Contract.Requires<ArgumentNullException>(entity != null, "entity");
            this.technique = technique;
            this.quad = quad;
            this.entity = entity;
            Inputs = new List<Texture>();
        }

        public override void PreRender()
        {
            base.PreRender();

            DirectXDevice device = DeviceService.DirectXDevice;
            device.SetCurrentEffect(Technique);
            device.InputAssembler.InputLayout = Technique.InputLayout;
            device.SetPixelShaderSampler(0, device.SamplerStates.Default);
            for (int i = 0; i < Inputs.Count; i++)
            {
                var texture = Inputs[i];
                device.SetPixelShaderShaderResourceView(i, texture.ShaderResourceView[ViewType.Full, 0, 0]);
            }

            foreach (Shader shader in Technique)
            {
                shader.UpdateBuffers(UpdateType.SceneStatic);
                shader.Apply(Technique.Name, UpdateType.SceneStatic);
            }
        }

        public override void Render()
        {
            DirectXDevice device = DeviceService.DirectXDevice;
            foreach (Shader shader in Technique)
                shader.Apply(Technique.Name, UpdateType.SceneFrame);

            foreach (Shader shader in Technique)
            {
                shader.Apply(Technique.Name, entity.Id, UpdateType.InstanceStatic);
                shader.Apply(Technique.Name, entity.Id, UpdateType.InstanceFrame);
            }

            quad.Draw(device);
        }


    }
}
