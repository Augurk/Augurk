using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Augurk.CommandLine.Commands
{
    [Export(typeof(CommandManager))]
    internal class CommandManager
    {
        [ImportMany]
        public IEnumerable<Lazy<ICommand, ICommandMetadata>> Commands;

        public void ExecuteCommand(string commandName)
        {
            // Find the command that implements the verb
            var command = Commands.FirstOrDefault(c => c.Metadata.Verb == commandName);
            if (command == null)
            {
                // Unknown verb
                return;
            }

            // Execute the command
            command.Value.Execute();
        }
    }
}
