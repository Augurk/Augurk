using Augurk.CommandLine.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Implements the delete command.
    /// </summary>
    [Export(typeof(ICommand))]
    [ExportMetadata("Verb", DeleteOptions.VERB_NAME)]
    internal class DeleteCommand : ICommand
    {
        private readonly DeleteOptions _options;

        [ImportingConstructor]
        public DeleteCommand(DeleteOptions options)
        {
            _options = options;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
