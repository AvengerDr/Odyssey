using System.Collections.Generic;
using Odyssey.Graphics.Organization.Commands;
using Odyssey.Organization.Commands;

namespace Odyssey.Talos.Messages
{
    public class OptimizationCompleteMessage : Message
    {
        private readonly LinkedList<Command> commands;
        public IEnumerable<Command> Commands { get { return commands; } }

        public OptimizationCompleteMessage(LinkedList<Command> commands, bool isSynchronous = false)
            : base(isSynchronous)
        {
            this.commands = commands;
        }
    }
}
