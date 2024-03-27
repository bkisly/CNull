using CNull.Common.Events.Args;

namespace CNull.Common.Mediators
{
    /// <summary>
    /// <inheritdoc cref="ICoreComponentsMediator"/>
    /// </summary>
    internal class CoreComponentsMediator : ICoreComponentsMediator
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event EventHandler<InputSourceRequestedEventArgs>? InputSourceRequested; 

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sourcePath"><inheritdoc/></param>
        public void NotifyInputSourceRequested(string sourcePath)
        {
            InputSourceRequested?.Invoke(this, new InputSourceRequestedEventArgs(sourcePath));
        }
    }
}
