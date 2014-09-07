using Odyssey.Graphics.Shaders;
using Odyssey.Talos.Messages;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Odyssey.Talos.Components
{
    [DebuggerDisplay("Key = {Key}", Name = "{AssetName}")]
    [YamlTag("Shader")]
    public sealed class ShaderComponent : ContentComponent, ITechniqueComponent
    {
        private string key;

        public ShaderComponent()
            : base(ComponentTypeManager.GetType<ShaderComponent>())
        {
            Key = "Default";
        }

        public override bool IsInited { get { return Technique != null; } }

        IEnumerable<Technique> ITechniqueComponent.Techniques
        {
            get { return new[] { Technique }; }
        }

        public string Key
        {
            get { return key; }
            set
            {
                if (string.Equals(key, value))
                    return;
                key = value;

                RaisePropertyChanged();
            }
        }

        public Technique Technique { get; private set; }

        public override void Initialize()
        {
            Contract.Requires<InvalidOperationException>(AssetName != null);
            ShaderCollection shaderCollection = Content.Load<ShaderCollection>(AssetName);

            var techniquePool = DeviceService.DirectXDevice.TechniquePool;
            var mapping = shaderCollection.Get(Key);
            string techniqueKey = string.Format("{0}.{1}", AssetName, mapping.Name);
            if (!techniquePool.ContainsTechnique(techniqueKey))
            {
                Technique = new Technique(DeviceService.DirectXDevice, techniqueKey, mapping);
                techniquePool.RegisterTechnique(Technique);
                Messenger.Send(new ContentMessage<Technique>(Owner, AssetName, Technique));
            }
            else Technique = techniquePool.GetTechnique(techniqueKey);
            
        }



    }


}