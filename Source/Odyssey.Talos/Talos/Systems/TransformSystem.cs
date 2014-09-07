#region Using Directives

using Odyssey.Engine;
using Odyssey.Talos.Components;
using SharpDX;
using SharpYaml.Serialization;

#endregion

namespace Odyssey.Talos.Systems
{
    [YamlTag("PositionSystem")]
    public class TransformSystem : UpdateableSystemBase
    {
        public TransformSystem()
            : base(Selector.All(typeof(PositionComponent), typeof(UpdateComponent), typeof(TransformComponent))
            .GetOne(typeof (RotationComponent), typeof (ScalingComponent), typeof (ParentComponent)))
        {
        }

        public override void Process(ITimeService time)
        {
            foreach (Entity entity in Entities)
            {
                if (!entity.IsEnabled)
                    continue;

                var cUpdate = entity.GetComponent<UpdateComponent>(Update.KeyPart);
                if (!cUpdate.RequiresUpdate)
                    continue;

                var cPosition = entity.GetComponent<PositionComponent>();
                var cTransform = entity.GetComponent<TransformComponent>();
                ScalingComponent cScaling;
                RotationComponent cRotation;
                ParentComponent cParent;

                Matrix mLocalWorld = Matrix.Identity;

                if (entity.TryGetComponent(out cScaling) && cScaling.Scale != Vector3.Zero)
                {
                    cTransform.Scaling = Matrix.Scaling(cScaling.Scale);
                    mLocalWorld *= cTransform.Scaling;
                }

                if (entity.TryGetComponent(out cRotation))
                {
                    if (cRotation.IsDirty || cTransform.Rotation.IsIdentity)
                    {
                        cTransform.Rotation = Matrix.RotationQuaternion(cRotation.Orientation);
                        cRotation.IsDirty = false;
                    }
                    mLocalWorld *= cTransform.Rotation;
                }

                if (cPosition.Position != Vector3.Zero)
                {
                    if (cPosition.IsDirty || cTransform.Translation.IsIdentity)
                    {
                        cTransform.Translation = Matrix.Translation(cPosition.Position);
                        cPosition.IsDirty = false;
                    }
                    mLocalWorld *= cTransform.Translation;
                }

                cTransform.Local = mLocalWorld;

                if (entity.TryGetComponent(out cParent) && cParent.Parent != null && cParent.Parent.ContainsComponent<TransformComponent>())
                {
                    var cParentTransform = cParent.Parent.GetComponent<TransformComponent>();
                    cTransform.World = cTransform.Local*cParentTransform.World;
                }
                else
                    cTransform.World = cTransform.Local;
            }
        }

        public override void AfterUpdate()
        {
            foreach (Entity entity in Scene.SystemMap.SelectAllEntities(this))
            {
                if (!entity.IsEnabled)
                    continue;

                var cUpdate = entity.GetComponent<UpdateComponent>();
                if (cUpdate.UpdateFrequency == UpdateFrequency.Static)
                    cUpdate.RequiresUpdate = false;
                else if (cUpdate.UpdateFrequency == UpdateFrequency.RealTime)
                    cUpdate.RequiresUpdate = true;
            }
        }
    }
}