using Odyssey.Engine;
using Odyssey.Geometry;
using Odyssey.Interaction;
using Odyssey.Talos.Components;
using SharpDX;
using System;

namespace Odyssey.Talos.Interaction
{
    public class WasdCameraController : ControllerBase
    {
        private CameraComponent camera;
        private IKeyboardService keyboardService;

        public WasdCameraController(IServiceRegistry services) :base(services)
        {
            MovementSpeed = 3.0f;
            StrafeSpeed = 3f;
            YawSpeed = MathHelper.Pi / 4;
        }

        public KeyBinding[] CameraBindings { get; set; }

        public float MovementSpeed { get; set; }

        public float StrafeSpeed { get; set; }

        public float YawSpeed { get; set; }

        public override void BindToEntity(Entity source)
        {
            base.BindToEntity(source);
            if (!source.TryGetComponent(out camera))
                throw new InvalidOperationException(string.Format("'{0}' does not contain a {1}", source.Name, camera.GetType()));

            keyboardService = Services.GetService<IKeyboardService>();

            if (CameraBindings == null)
                CameraBindings = new[]
                {
                    new KeyBinding(Keys.W, CameraAction.MoveForward, ButtonStateFlags.Down),
                    new KeyBinding(Keys.S, CameraAction.MoveBackward, ButtonStateFlags.Down),
                    new KeyBinding(Keys.A, CameraAction.StrafeLeft, ButtonStateFlags.Down),
                    new KeyBinding(Keys.D, CameraAction.StrafeRight, ButtonStateFlags.Down),
                    new KeyBinding(Keys.Q, CameraAction.HoverDown, ButtonStateFlags.Down),
                    new KeyBinding(Keys.E, CameraAction.HoverUp, ButtonStateFlags.Down),
                    new KeyBinding(Keys.Z, CameraAction.YawLeft, ButtonStateFlags.Down),
                    new KeyBinding(Keys.C, CameraAction.YawRight, ButtonStateFlags.Down)
                };
        }

        public override void Update(ITimeService time)
        {
            if (!CUpdate.RequiresUpdate)
                return;

            KeyboardState keyboard = keyboardService.GetState();

            foreach (var keyBinding in CameraBindings)
            {
                ButtonState state = keyboard[keyBinding.Key];
                if (state.Flags.HasFlag(keyBinding.Trigger))
                    KeyDown(keyBinding, time);
            }
        }

        protected virtual void KeyDown(KeyBinding binding, ITimeService time)
        {
            float elapsedTime = (float)time.ElapsedApplicationTime.TotalSeconds;
            switch (binding.ActionType)
            {
                case CameraAction.MoveForward:
                    Move(camera.Direction, MovementSpeed * elapsedTime);
                    break;

                case CameraAction.MoveBackward:
                    Move(camera.Direction, -MovementSpeed * elapsedTime);
                    break;

                case CameraAction.StrafeLeft:
                    Move(Vector3.Cross(camera.Direction, camera.Up), -StrafeSpeed * elapsedTime);
                    break;

                case CameraAction.StrafeRight:
                    Move(Vector3.Cross(camera.Direction, camera.Up), StrafeSpeed * elapsedTime);
                    break;

                case CameraAction.HoverUp:
                    Move(camera.Up, MovementSpeed * elapsedTime);
                    break;

                case CameraAction.HoverDown:
                    Move(camera.Up, -MovementSpeed * elapsedTime);
                    break;

                case CameraAction.YawLeft:
                    Rotate(camera.Up, -YawSpeed * elapsedTime);
                    break;

                case CameraAction.YawRight:
                    Rotate(camera.Up, YawSpeed * elapsedTime);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("binding.ActionType");
            }
        }

        protected void Move(Vector3 direction, float distance)
        {
            CPosition.Position += direction * MovementSpeed * distance;
        }

        protected void Rotate(Vector3 axis, float angle)
        {
            CRotation.Orientation *= Quaternion.RotationAxis(axis, angle);
        }
    }
}