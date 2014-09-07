using Odyssey.Graphics;
using Odyssey.Graphics.Organization;
using SharpDX;

namespace Odyssey.Organization.Commands
{
    public class DepthStencilStateChangeCommand : EngineCommand
    {
        private readonly DepthStencilState depthStencilState;

        public DepthStencilState DepthStencilState
        {
            get { return depthStencilState; }
        }

        public DepthStencilStateChangeCommand(IServiceRegistry services, DepthStencilState depthStencilState)
            : base(services, CommandType.DepthStencilStateChange)
        {
            this.depthStencilState = ToDispose(depthStencilState);
        }

        public override void Initialize()
        {
            depthStencilState.Initialize();
            IsInited = true;
        }

        public override void Execute()
        {
            DeviceService.DirectXDevice.SetDepthStencilState(depthStencilState);
        }

        
    }
}
