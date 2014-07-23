﻿using SharpDX;

namespace Odyssey.Graphics.Organization.Commands
{
    public class RasterizerStateChangeCommand : Command
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