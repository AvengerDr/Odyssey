using System;
using System.Collections.Generic;
using System.Linq;
using Odyssey.Engine;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos.Nodes;
using SharpDX;

namespace Odyssey.Talos.Initializers
{
    internal class LightInitializer : Initializer<LightNode>
    {

        public LightInitializer(IServiceRegistry services)
            : base(services, Reference.Group.Light)
        {
        } 

        public override void SetupInitialization(ShaderInitializer initializer)
        {
            var services = initializer.Services;
            var technique = initializer.Technique;
            InitializerParameters parameters = new InitializerParameters(-1, technique, services, StaticSelector);
            var data = from metaData in technique.MetaData
                where metaData.Key == Param.Properties.LightId
                select Int32.Parse(metaData.Value);

            ILightService lightService = services.GetService<ILightService>();

            foreach (int lightId in data)
                initializer.Initialize(this, lightService[lightId], parameters);
        }

        protected override bool ValidateConstantBuffer(ConstantBufferDescription cb)
        {
            return cb.ContainsMetadata(Param.Properties.LightId);
        }

        private static readonly Dictionary<string, ParameterMethod> ReferenceActions = new Dictionary<string, ParameterMethod>
        {
            {
                Reference.Struct.PointLightPS, (index, light, parameters) => Convert(index, (PointLightNode)light)
            },
            {
                Reference.Vector.Direction, (index, light, parameters) => new[]
                {new Float3Parameter(index, "lightdir", () => new Vector3(-1, 0, -1))}
            },
        };

        protected override IEnumerable<IParameter> CreateParameter(ConstantBufferDescription cbParent,LightNode lightNode, int parameterIndex, string reference, InitializerParameters initializerParameters)
        {
            string lightIdValue = cbParent.Get(Param.Properties.LightId);
            int lightId = Int32.Parse(lightIdValue);

            if (!ReferenceActions.ContainsKey(reference))
                throw new InvalidOperationException(string.Format("[{0}]: Light parameter not valid", reference));

            return ReferenceActions[reference](parameterIndex, lightNode, initializerParameters);
        }

        static IEnumerable<IParameter> Convert(int parameterIndex, PointLightNode lightNode)
        {
            var cLight = lightNode.LightComponent;

            List<IParameter> parameterCollection = new List<IParameter>
            {
                new Float4Parameter(parameterIndex, Param.Light.Params,
                    () => new Vector4(cLight.Intensity, cLight.Range, 0, 0)),
                new Float4Parameter(parameterIndex, Param.Light.Diffuse, () => cLight.Diffuse.ToVector4()),
                new Float3Parameter(parameterIndex, Param.Light.Position, () => lightNode.WorldPosition)
            };
            return parameterCollection;
        }

    }
}
