using CNull.Common.Events;

namespace CNull.Common.Mediators
{
    /// <summary>
    /// Mediator which enables independent communication between different core modules.
    /// </summary>
    public interface ICoreComponentsMediator
    {
        /// <summary>
        /// Returns the path to the currently processed source.
        /// </summary>
        string CurrentSourcePath { get; }

        /// <summary>
        /// Event raised when input source has been requested.
        /// </summary>
        event EventHandler<InputRequestedEventArgs> InputRequested;

        /// <summary>
        /// Raises FileInputRequested event.
        /// </summary>
        /// <param name="reader">Lazy initializer of the reader from which code will be read.</param>
        /// <param name="path">Formatted path to the source.</param>
        void NotifyInputRequested(Lazy<TextReader> reader, string path);
    }
}
