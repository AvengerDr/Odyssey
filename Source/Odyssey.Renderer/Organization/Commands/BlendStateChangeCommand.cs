using Odyssey.Graphics;
using Odyssey.Graphics.Organization;
using SharpDX;

namespace Odyssey.Organization.Commands
{
    public class BlendStateChangeCommand : EngineCommand
    {
        private readonly BlendState blendState;

        public BlendState BlendState
        {
            get { return blendState; }
        }

        public BlendStateChangeCommand(IServiceRegistry services, BlendState blendState)
            : base(services, CommandType.BlendStateChange)
        {
            this.blendState = ToDispose(blendState);
        }

        public override void Initialize()
        {
            blendState.Initialize();
            IsInited = true;
        }

        public override void Execute()
        {
            DeviceService.DirectXDevice.SetBlendState(blendState);
        }
       
    }
}
