namespace Augurk.CommandLine.Commands
{
    /// <summary>
    /// Describes the required metadata for a command.
    /// </summary>
    public interface ICommandMetadata
    {
        /// <summary>
        /// Gets the verb of the command.
        /// </summary>
        string Verb { get; }
    }
}
