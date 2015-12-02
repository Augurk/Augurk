using System;
using Augurk.CommandLine.Commands;
using Augurk.CommandLine.Options;
using CommandLine;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.ComponentModel.Composition;

namespace Augurk.CommandLine
{
    /// <summary>
    /// Entry-point for the command line tool.
    /// </summary>
    static class Program
    {

        /// <summary>
        /// Called when the command line tool starts.
        /// </summary>
        /// <param name="args">Arguments for the application.</param>
        static void Main(string[] args)
        {
            // Parse the command line arguments
            int exitCode = 0;
            var options = new GlobalOptions();
            using (var parser = new Parser(settings => { settings.MutuallyExclusive = true; settings.HelpWriter = Console.Error; }))
            {
                if (!parser.ParseArguments(args, options, ExecuteVerb))
                {
                    exitCode = Parser.DefaultExitCodeFail;
                }
            }

#if DEBUG
            Console.ReadLine();
#endif
            Environment.Exit(exitCode);
        }


        /// <summary>
        /// Called to execute a specific supported verb.
        /// </summary>
        /// <param name="verbName">Name of the verb to execute.</param>
        /// <param name="verbInstance">Instance of the options for the verb.</param>
        private static void ExecuteVerb(string verbName, object verbInstance)
        {
            // Make sure that we have a verb instance
            if (verbInstance == null)
            {
                // No verb instance available, so parsing command line arguments failed, nothing more to do
                return;
            }

            // Create the container so that it dynamically determines the commands
            using (var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly()))
            using (var container = new CompositionContainer(catalog))
            {
                try
                {
                    // Inject the options instance into the container
                    container.ComposeExportedValue(verbInstance);

                    // Get the command manager from the container and let it execute the command
                    var commandManager = container.GetExportedValue<CommandManager>();
                    commandManager.ExecuteCommand(verbName);
                }
                catch (CompositionException compositionException)
                {
                    Console.WriteLine(compositionException.ToString());
                }
            }
        }
    }
}
