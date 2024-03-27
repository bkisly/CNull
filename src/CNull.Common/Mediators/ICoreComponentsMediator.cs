using CNull.Common.Events.Args;

namespace CNull.Common.Mediators
{
    /// <summary>
    /// Mediator which enables independent communication between different core modules.
    /// </summary>
    public interface ICoreComponentsMediator
    {
        /// <summary>
        /// Event raised when input source has been given.
        /// </summary>
        event EventHandler<InputSourceRequestedEventArgs> InputSourceRequested;

        /// <summary>
        /// Raises InputSourceRequested event.
        /// </summary>
        /// <param name="sourcePath">Path to the input source.</param>
        void NotifyInputSourceRequested(string sourcePath);
    }
}
