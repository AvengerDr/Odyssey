using System.Diagnostics;
using Odyssey.Animations;
using SharpDX;

namespace Odyssey.Talos.Components
{
    [DebuggerDisplay("{Position}: ({Position})")]
    public class PositionComponent : Component, IPosition
    {
        private Vector3 position;

        [Animatable]
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                IsDirty = true;
            }
        }

        internal bool IsDirty { get; set; }

        public PositionComponent()
            : base(ComponentTypeManager.GetType<PositionComponent>())
        {
            Position = Vector3.Zero;
        }

    }
}
