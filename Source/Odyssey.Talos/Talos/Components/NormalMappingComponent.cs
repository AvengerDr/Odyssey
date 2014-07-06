﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Odyssey.Graphics;
using Odyssey.Graphics.Effects;
using Odyssey.Utilities.Logging;
using SharpYaml.Serialization;

namespace Odyssey.Talos.Components
{
    [RequiredComponent(typeof(ShaderComponent))]
    [YamlTag("NormalMapping")]
    public class NormalMappingComponent : DiffuseMappingComponent
    {
        public override bool IsInited
        {
            get { return base.IsInited && NormalMap != null && NormalMap.IsInited;  }
        }

        public Texture NormalMap { get; private set; }
        public string NormalMapKey { get; set; }

        public override void Unload()
        {
            base.Unload();
            if (NormalMap != null)
                NormalMap.Unload();
        }

        public override void Initialize()
        {
            Contract.Requires<InvalidOperationException>(NormalMapKey != null);
            base.Initialize();
            NormalMap = Content.Get<Texture>(NormalMapKey);
            if (!NormalMap.IsInited)
                NormalMap.Initialize();
            TextureMap.Add(TextureReference.NormalMap, NormalMap);
        }

        public override bool Validate()
        {
            bool test = Content.Contains(NormalMapKey);
            if (!test)
                LogEvent.Engine.Error("[{0}] not found.", NormalMapKey);
            return base.Validate() && test;
        }

    }
}