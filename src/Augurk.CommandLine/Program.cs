using System;
using Augurk.CommandLine.Commands;
using Augurk.CommandLine.Options;
using CommandLine;

namespace Augurk.CommandLine
{
    /// <summary>
    /// Entry-point for the command line tool.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Called when the command line tool starts.
        /// </summary>
        /// <param name="args">Arguments for the application.</param>
        static void Main(string[] args)
        {
            // Parse the command line options
            var exitCode = 0;
            var options = new GlobalOptions();
            using (var parser = new Parser(settings => { settings.MutuallyExclusive = true; settings.HelpWriter = Console.Error; }))
            {
                if (!parser.ParseArguments(args, options, ExecuteVerb))
                {
                    exitCode = Parser.DefaultExitCodeFail;
                }
            }

            // Command line arguments not succesfully parsed
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
        static void ExecuteVerb(string verbName, object verbInstance)
        {
            // Make sure that we have a verb instance
            if (verbInstance == null)
            {
                // No verb instance available, so parsing command line arguments failed, nothing more to do
                return;
            }

            // Determine the verb that was executed
            switch (verbName)
            {
                case PublishOptions.VERB_NAME:
                    var verbOptions = (PublishOptions)verbInstance;
                    var command = new PublishCommand(verbOptions);
                    command.Execute();
                    return;
                default:
                    // Unknown verb
                    return;
            }
        }
    }
}
