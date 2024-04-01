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
        public event EventHandler<FileInputRequestedEventArgs>? FileInputRequested; 

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sourcePath"><inheritdoc/></param>
        public void NotifyFileInputRequested(string sourcePath)
        {
            FileInputRequested?.Invoke(this, new FileInputRequestedEventArgs(sourcePath));
        }
    }
}
