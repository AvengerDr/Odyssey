using Odyssey.Engine;
using Odyssey.Geometry;
using Odyssey.Talos;
using SharpDX;

namespace Odyssey.UserInterface.Behaviors
{
    public class AnchorBehaviour : Behavior<UIElement>
    {
        private static readonly Vector3 Epsilon = new Vector3(MathHelper.Epsilon);
        private readonly IPosition positionProvider;
        private readonly ICamera cameraProvider;
        private Vector3 lastPosition;

        public Vector2 Offset { get; set; }

        public AnchorBehaviour(IPosition positionProvider, ICamera cameraProvider)
        {
            this.positionProvider = positionProvider;
            this.cameraProvider = cameraProvider;
        }

        protected override void OnAttached()
        {
            AssociatedElement.Tick += OnTick;
        }

        void OnTick(object sender, TimeEventArgs e)
        {
            var viewport = cameraProvider.Viewport;
            var currentPosition = Vector3.Project(positionProvider.Position, viewport.X, viewport.Y, viewport.Width, viewport.Height,
                viewport.MinDepth, viewport.MinDepth, cameraProvider.View*cameraProvider.Projection);

            if (!Vector3.NearEqual(currentPosition, lastPosition, Epsilon))
            {
                AssociatedElement.Position = new Vector2(lastPosition.X, lastPosition.Y) + Offset;
                lastPosition = currentPosition;
            }
        }
    }
}
