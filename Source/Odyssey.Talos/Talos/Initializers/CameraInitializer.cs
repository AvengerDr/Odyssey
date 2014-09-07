using System;
using System.Collections.Generic;
using System.Linq;
using Odyssey.Engine;
using Odyssey.Graphics.Effects;
using Odyssey.Graphics.Shaders;
using Odyssey.Talos.Components;
using Odyssey.Talos.Nodes;
using SharpDX;
using Buffer = Odyssey.Graphics.Buffer;
using EngineReference = Odyssey.Graphics.Effects.EngineReference;

namespace Odyssey.Talos.Initializers
{
    internal class CameraInitializer: Initializer<Entity>
    {

        public CameraInitializer(IServiceRegistry services)
            : base(services, Reference.Group.Camera)
        {
        } 

        public override void SetupInitialization(ShaderInitializer initializer)
        {
            var services = initializer.Services;
            var scene = services.GetService<IEntityProvider>();
            var technique = initializer.Technique;
            InitializerParameters parameters = new InitializerParameters(-1, technique, services, StaticSelector);
            var data = from metaData in technique.MetaData
                       where metaData.Key == Param.Properties.CameraId
                       select Int32.Parse(metaData.Value);

            var cameras = (from e in scene.Entities
                where e.ContainsComponent<CameraComponent>()
                let cameraComponent = e.GetComponent<CameraComponent>()
                select new {Entity = e, CameraComponent = cameraComponent}).ToDictionary(kvp=> kvp.CameraComponent.Index, kvp => kvp.Entity);

            foreach (int cameraId in data)
                initializer.Initialize(this, cameras[cameraId], parameters);
        }

        protected override bool ValidateConstantBuffer(ConstantBufferDescription cb)
        {
            return cb.ContainsMetadata(Param.Properties.CameraId);
        }

        protected override IEnumerable<IParameter> CreateParameter(ConstantBufferDescription cbParent, Entity entity, int parameterIndex, string reference, InitializerParameters initializerParameters)
        {
            string cameraIdValue = cbParent.Get(Param.Properties.CameraId);
            int cameraId = Int32.Parse(cameraIdValue);

            if (!ReferenceActions.ContainsKey(reference))
                throw new InvalidOperationException(string.Format("[{0}]: Camera parameter not valid", reference));

            return ReferenceActions[reference](parameterIndex, entity, initializerParameters);
        }

        private static readonly Dictionary<string, ParameterMethod> ReferenceActions = new Dictionary<string, ParameterMethod>
        {
            {
                Reference.Vector.Position, (index, entity, parameters) => new[]
                {new Float3Parameter(index, Param.Vectors.CameraPosition, () => entity.GetComponent<PositionComponent>().Position)}
            },
            {
                Reference.Matrix.View, (index, entity, parameters) => new[]
                {new MatrixParameter(index, Param.Matrices.View, () => entity.GetComponent<CameraComponent>().View)}
            },
            {
                Reference.Matrix.Projection, (index, entity, parameters) => new[]
                {new MatrixParameter(index, Param.Matrices.Projection, () => entity.GetComponent<CameraComponent>().Projection)}
            },

        };

    }
}
