#region License

// Copyright © 2013-2014 Avengers UTD - Adalberto L. Simeone
// 
// The Odyssey Engine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License Version 3 as published by
// the Free Software Foundation.
// 
// The Odyssey Engine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details at http://gplv3.fsf.org/

#endregion

#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Odyssey.Engine;
using Odyssey.Graphics;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos.Components;
using Odyssey.Utilities.Logging;
using SharpDX;

#endregion

namespace Odyssey.Talos.Initializers
{
    public class EntityInitializer : Initializer<Entity>
    {
        private static readonly Dictionary<string, ParameterMethod> ReferenceActions = new Dictionary<string, ParameterMethod>
        {
            {
                Reference.Matrix.World, (index, entity, parameters) => new[]
                {new MatrixParameter(index, Param.Matrices.World, () => entity.GetComponent<TransformComponent>().World)}
            },
            {
                Reference.Matrix.WorldInverse, (index, entity, parameters) => new[]
                {
                    new MatrixParameter(index, Param.Matrices.WorldInverse,
                        () => Matrix.Invert(entity.GetComponent<TransformComponent>().World))
                }
            },
            {
                Reference.Matrix.WorldInverseTranspose, (index, entity, parameters) => new[]
                {
                    new MatrixParameter(index, Param.Matrices.WorldInverseTranspose,
                        () => Matrix.Transpose(Matrix.Invert(entity.GetComponent<TransformComponent>().World)))
                }
            },
        };

        public EntityInitializer(IServiceRegistry services)
            : base(services, Reference.Group.Entity)
        {
        }

        public override void Initialize(DirectXDevice device, Entity source, InitializerParameters parameters)
        {
            base.Initialize(device, source, parameters);
            if (!parameters.Technique.UsesProceduralTextures)
                InitializeTextures(source, parameters);
        }

        public override void SetupInitialization(ShaderInitializer initializer)
        {
            var services = initializer.Services;
            var technique = initializer.Technique;

            var scene = services.GetService<IEntityProvider>();

            var data = from e in scene.Entities
                let techniqueComponents = e.Components.OfType<ITechniqueComponent>()
                from cTechnique in techniqueComponents
                from t in cTechnique.Techniques
                where t.Name == technique.Name
                select e.Id;

            foreach (long entityId in data)
            {
                InitializerParameters parameters = new InitializerParameters(entityId, technique, services, InstanceSelector);
                initializer.Initialize(this, scene.SelectEntity(entityId), parameters);
            }
        }

        protected override IEnumerable<IParameter> CreateParameter(ConstantBufferDescription cbParent, Entity entity, int parameterIndex,
            string reference, InitializerParameters initializerParameters)
        {
            if (!ReferenceActions.ContainsKey(reference))
                throw new InvalidOperationException(string.Format("[{0}]: Entity parameter not valid", reference));

            return ReferenceActions[reference](parameterIndex, entity, initializerParameters);
        }

        protected override bool ValidateConstantBuffer(ConstantBufferDescription cb)
        {
            return true;
        }

        /// <summary>
        /// Searches through components looking for a <see cref="Texture"/> resource associated to
        /// a <see cref="Shader"/> asset having the specified reference.
        /// </summary>
        /// <param name="components">A sequence of <see cref="Components.Component"/> belonging to an <see cref="Entity"/>.</param>
        /// <param name="key">The key value identifying the <see cref="Shader"/> asset.</param>
        /// <param name="reference">The reference value identifying the <see cref="Texture"/>.</param>
        /// <returns></returns>
        private static Texture FindResource(IEnumerable<IComponent> components, string key, string reference)
        {
            var enumerable = components as IComponent[] ?? components.ToArray();
            var resource = enumerable.OfType<ITextureResource>().FirstOrDefault(c => c.AssetName == key);

            return resource != null ? resource[reference] : null;
        }


        private void InitializeTextures(IEntity source, InitializerParameters parameters)
        {
            var technique = parameters.Technique;
            var referenceTable = from shaderObject in technique.Mapping.Shaders
                from textures in shaderObject.TextureReferences
                select new {Shader = shaderObject, TextureDesc = textures};

            foreach (var row in referenceTable)
            {
                var textureDesc = row.TextureDesc;
                if (!technique[textureDesc.ShaderType].HasTexture(textureDesc.Index))
                {
                    Texture texture = FindResource(source.Components, row.Shader.Name, row.TextureDesc.Texture);
                    if (texture == null)
                    {
                        LogEvent.Engine.Error("[{0}] is missing a component containing a [{1}] texture and tagged with [{2}].", source.Name,
                            textureDesc.Texture, row.Shader.Name);
                        throw new InvalidOperationException("No suitable textures found.");
                    }
                    technique[textureDesc.ShaderType].AddTexture(new TextureMapping(texture, textureDesc));
                }
            }
        }
    }
}