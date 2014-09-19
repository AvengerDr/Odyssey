using System.Collections.Generic;
using Odyssey.Organization.Commands;

namespace Odyssey.Talos.Messages
{
    public class CommandUpdateMessage : Message
    {
        public IEnumerable<Command> Commands { get; private set; }
        public UpdateType UpdateType { get; private set; }

        public CommandUpdateMessage(Command command, UpdateType updateType = UpdateType.Add, bool isSynchronous = false)
            : this(new[] { command}, updateType, isSynchronous)
        { }

        public CommandUpdateMessage(IEnumerable<Command> commands, UpdateType updateType = UpdateType.Add, bool isSynchronous = false)
            : base(isSynchronous)
        {
            UpdateType = updateType;
            Commands = commands;
        }
    }
}
