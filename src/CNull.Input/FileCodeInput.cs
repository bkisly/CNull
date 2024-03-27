using CNull.Common.Events.Args;
using CNull.Common.Mediators;

namespace CNull.Input
{
    /// <summary>
    /// Class for handling code input from files.
    /// </summary>
    internal class FileCodeInput : ICodeInput, IDisposable
    {
        private StreamReader? _stream;
        private readonly ICoreComponentsMediator _mediator;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public char? CurrentCharacter { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="FileCodeInput"/>.
        /// </summary>
        /// <param name="mediator">Mediator needed to receive an event about requested input source.</param>
        public FileCodeInput(ICoreComponentsMediator mediator)
        {
            _mediator = mediator;
            _mediator.InputSourceRequested += OnInputSourceRequested;
        }

        private void OnInputSourceRequested(object? sender, InputSourceRequestedEventArgs e)
        {
            try
            {
                _stream = new StreamReader(e.SourcePath);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void MoveToNext()
        {
            if (_stream == null)
                throw new NotImplementedException("Implement error when stream is not initialized and character is advanced.");

            var character = _stream.Read();
            CurrentCharacter = character == -1 ? null : (char)character;
        }

        /// <summary>
        /// Cleans up resources related to the file stream.
        /// </summary>
        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
