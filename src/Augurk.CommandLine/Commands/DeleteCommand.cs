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
    internal class DeleteCommand : ICommand
    {
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
