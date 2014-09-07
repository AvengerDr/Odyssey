using System;
using System.Collections.Generic;
using System.Linq;
using Odyssey.Engine;
using Odyssey.Graphics.Effects;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos.Components;
using SharpDX;

namespace Odyssey.Talos.Initializers
{
    public class MaterialInitializer : Initializer<IMaterial>
    {

        public MaterialInitializer(IServiceRegistry services)
            : base(services, Reference.Group.Material)
        {
        } 

        public override void SetupInitialization(ShaderInitializer initializer)
        {
            var services = initializer.Services;
            var technique = initializer.Technique;
            
            var scene = services.GetService<IEntityProvider>();
            var data = from e in scene.Entities
                where e.ContainsComponent<MaterialComponent>()
                let techniqueComponents = e.Components.OfType<ITechniqueComponent>()
                from techniqueRange in techniqueComponents
                from t in techniqueRange.Techniques
                       where t.Name == technique.Name
                group e.Id by e.GetComponent<MaterialComponent>()
                into materialEntities
                select new {Material = materialEntities.Key, Entities = materialEntities.ToArray()};

            foreach (var kvp in data)
            {
                foreach (var entityId in kvp.Entities)
                {
                    InitializerParameters parameters = new InitializerParameters(entityId, technique, services, InstanceSelector);
                    initializer.Initialize(this, (IMaterial) kvp.Material, parameters);
                }
            }
        }

        protected override bool ValidateConstantBuffer(ConstantBufferDescription cb)
        {
            return cb.ContainsMetadata(Param.Properties.Material);
        }

        protected override IEnumerable<IParameter> CreateParameter(ConstantBufferDescription cbParent, IMaterial material, int parameterIndex, string reference, InitializerParameters initializerParameters)
        {
            if (!ReferenceActions.ContainsKey(reference))
                throw new InvalidOperationException(string.Format("[{0}]: Material parameter not valid", reference));

            return ReferenceActions[reference](parameterIndex, material, initializerParameters);
        }

        private static readonly Dictionary<string, ParameterMethod> ReferenceActions = new Dictionary<string, ParameterMethod>
        {
            {
                Reference.Color.Diffuse,
                (index, material,parameters) => new[] {new Float4Parameter(index, Param.Material.Diffuse, () => material.Diffuse)}
            },
            {Reference.Struct.MaterialPS, Convert}
        };

        static IEnumerable<IParameter> Convert(int parameterIndex, IMaterial material, InitializerParameters parameters)
        {
            List<IParameter> parameterCollection = new List<IParameter>
            {
                new Float4Parameter(parameterIndex, Param.Material.Coefficients, () =>
                    new Vector4(material.AmbientCoefficient,
                        material.DiffuseCoefficient,
                        material.SpecularCoefficient,
                        material.SpecularPower)),
                new Float4Parameter(parameterIndex, Param.Material.Ambient, () => material.Ambient),
                new Float4Parameter(parameterIndex, Param.Material.Diffuse, () => material.Diffuse),
                new Float4Parameter(parameterIndex, Param.Material.Specular, () => material.Specular)
            };
            return parameterCollection;
        }

    }
}
