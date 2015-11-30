using Augurk.CommandLine.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Describes the interface for executable commands.
    /// </summary>
    internal interface ICommand
    {
        /// <summary>
        /// Called when the command should be executed.
        /// </summary>
        void Execute();
    }
}
