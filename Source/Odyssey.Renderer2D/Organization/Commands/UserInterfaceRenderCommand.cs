using Odyssey.Graphics.Organization;
using Odyssey.UserInterface.Controls;
using SharpDX;

namespace Odyssey.Organization.Commands
{
    public class UserInterfaceRenderCommand : Command
    {
        private readonly Overlay overlay;

        public UserInterfaceRenderCommand(IServiceRegistry services, Overlay overlay) : base(services, CommandType.Render2D)
        {
            this.overlay = overlay;
        }

        public override void Initialize()
        {
            IsInited = true;
        }

        public override void Execute()
        {
            overlay.Display();
        }
    }
}
