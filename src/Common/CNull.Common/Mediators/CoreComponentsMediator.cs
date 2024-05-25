﻿using CNull.Common.Events;

namespace CNull.Common.Mediators
{
    /// <summary>
    /// <inheritdoc cref="ICoreComponentsMediator"/>
    /// </summary>
    public class CoreComponentsMediator : ICoreComponentsMediator
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string CurrentSourcePath { get; private set; } = "<unknown>";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public event EventHandler<InputRequestedEventArgs>? InputRequested;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="reader"><inheritdoc/></param>
        /// <param name="path"><inheritdoc/></param>
        public void NotifyInputRequested(Lazy<TextReader> reader, string path)
        {
            CurrentSourcePath = Path.GetFullPath(path);
            InputRequested?.Invoke(this, new InputRequestedEventArgs(reader, path));
        }
    }
}
