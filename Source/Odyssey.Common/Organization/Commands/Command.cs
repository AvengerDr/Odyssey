using System;
using System.Diagnostics.Contracts;
using Odyssey.Graphics.Organization;
using SharpDX;

namespace Odyssey.Organization.Commands
{
    public abstract class Command : Component, ICommand
    {
        private readonly CommandType type;
        private readonly IServiceRegistry services;

        protected IServiceRegistry Services { get { return services; } }

        public bool IsInited { get; protected set; }

        public CommandType Type { get { return type; } }

        public abstract void Initialize();
        public abstract void Execute();

        protected Command(IServiceRegistry services, CommandType type)
        {
            Contract.Requires<ArgumentNullException>(services != null, "services");
            this.services = services;
            this.type = type;
        }

    }
}