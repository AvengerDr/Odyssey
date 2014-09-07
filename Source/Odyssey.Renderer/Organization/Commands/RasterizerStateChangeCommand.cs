using Odyssey.Graphics;
using Odyssey.Graphics.Organization;
using SharpDX;

namespace Odyssey.Organization.Commands
{
    public class RasterizerStateChangeCommand : EngineCommand
    {
        private readonly RasterizerState rasterizerState;
        public RasterizerState RasterizerState { get { return rasterizerState; } }

        public RasterizerStateChangeCommand(IServiceRegistry services, RasterizerState rasterizerState)
            : base(services, CommandType.RasterizerStateChange)
        {
            this.rasterizerState = ToDispose(rasterizerState);
            
        }

        public override void Initialize()
        {
            rasterizerState.Initialize();
            IsInited = true;
        }

        public override void Execute()
        {
            DeviceService.DirectXDevice.SetRasterizerState(rasterizerState);
        }
    }
}