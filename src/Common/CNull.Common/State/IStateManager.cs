using CNull.Common.Events;

namespace CNull.Common.State
{
    /// <summary>
    /// Stores information about the current state of the input of the processed program modules.
    /// </summary>
    public interface IStateManager
    {
        /// <summary>
        /// Returns the path to the currently processed source.
        /// </summary>
        string CurrentSourcePath { get; }

        /// <summary>
        /// Returns the name of the currently processed module.
        /// </summary>
        string CurrentModuleName { get; }

        /// <summary>
        /// Event raised when input source has been requested.
        /// </summary>
        event EventHandler<InputRequestedEventArgs> InputRequested;

        /// <summary>
        /// Notifies core components to read from a new file source.
        /// </summary>
        /// <param name="path">Path to the file to be processed.</param>
        void NotifyInputRequested(string path);

        /// <summary>
        /// Notifies core components to read from the specified stream. This method does not support multi-moduled programs.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        void NotifyInputRequested(Lazy<Stream> stream);

        /// <summary>
        /// Tries to open a program, which is under specified module name.
        /// </summary>
        /// <param name="moduleName">Module to enter.</param>
        /// <returns><see langword="true"/> if successfully opened, <see langword="false"/> otherwise.</returns>
        bool TryOpenModule(string moduleName);
    }
}
