using System;
using System.Collections.Generic;
using System.Linq;
using Odyssey.Engine;
using Odyssey.Geometry;
using Odyssey.Graphics.PostProcessing;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos.Components;
using SharpDX;

namespace Odyssey.Talos.Initializers
{
    internal class EffectInitializer : Initializer<Entity>
    {
        private readonly IDirectXDeviceSettings deviceSettings;

        public EffectInitializer(IServiceRegistry services)
            : base(services, Reference.Group.Effect)
        {
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

        protected override bool ValidateConstantBuffer(ConstantBufferDescription cb)
        {
            return true;
        }

        protected override IEnumerable<IParameter> CreateParameter(ConstantBufferDescription cbParent, Entity entity, int parameterIndex, string reference, InitializerParameters initializerParameters)
        {
            if (!ReferenceActions.ContainsKey(reference))
                throw new InvalidOperationException(string.Format("[{0}]: Effect parameter not valid", reference));

            return ReferenceActions[reference](parameterIndex, entity, initializerParameters);
        }

        private static readonly Dictionary<string, ParameterMethod> ReferenceActions = new Dictionary<string, ParameterMethod>
        {
            {
                Reference.Effect.BlurOffsetWeights, ComputeBlurOffsetsAndWeights
            },
            {
                Reference.Effect.BloomThreshold, (index, entity, parameters) => new[]
                {
                    new FloatParameter(index, Param.Floats.BloomThreshold,
                        () => entity.GetComponent<BloomComponent>().Threshold),
                }
            },
            {
                Reference.Effect.BloomParameters, (index, entity, parameters) => new[]
                {
                    new Float4Parameter(index, "BloomParameters",
                        delegate
                        {
                            var cBloom = entity.GetComponent<BloomComponent>();
                            return new Vector4(cBloom.Intensity, cBloom.BaseIntensity, cBloom.Saturation, cBloom.BaseSaturation);
                        })
                }
            },
            {
                Reference.Effect.GlowStrength, (index, entity, parameters) => new[]
                {
                    new FloatParameter(index, Param.Floats.GlowStrength,
                        () => entity.GetComponent<GlowComponent>().GlowStrength),
                }
            },
            {
                Reference.Effect.SpritePosition, (index, entity, parameters) => new[]
                {
                    new Float3Parameter(index, Param.Vectors.SpritePosition,
                        () => entity.GetComponent<SpriteComponent>().Position.ToVector3(1)),
                }
            },
            {
                Reference.Effect.SpriteSize, (index, entity, parameters) => new[]
                {
                    new Float2Parameter(index, Param.Vectors.SpriteSize,
                        () => entity.GetComponent<SpriteComponent>().Size),
                }
            },
        };

        private static IEnumerable<IParameter> ComputeBlurOffsetsAndWeights(int index, Entity entity, InitializerParameters parameters)
        {
            var deviceSettings = parameters.Services.GetService<IDirectXDeviceSettings>();
            bool isHorizontal = parameters.Technique.Name == "PostProcess.GaussianBlurH";
            float texelWidth;
            float texelHeight;
            var cBlur = entity.GetComponent<BlurComponent>();
            if (isHorizontal)
            {
                texelWidth = 1.0f/(deviceSettings.PreferredBackBufferWidth*cBlur.DownScale);
                texelHeight = 0;
            }
            else
            {
                texelWidth = 0;
                texelHeight = 1.0f/(deviceSettings.PreferredBackBufferHeight*cBlur.DownScale);
            }
            Vector2[] offsets;
            float[] weights;
            Blur.ComputeParameters(texelWidth, texelHeight, out weights, out offsets);
            Vector4[] data = new Vector4[Blur.SampleCount];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new Vector4(offsets[i].X, offsets[i].Y, weights[i], 0);
            }
            return new[] {new Float4ArrayParameter(index, data.Length, Param.Floats.BlurOffsetsAndWeights, () => data)};
        }

    }
}
