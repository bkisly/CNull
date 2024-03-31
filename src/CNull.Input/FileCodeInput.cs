using CNull.Common.Events.Args;
using CNull.Common.Mediators;

namespace CNull.Input
{
    /// <summary>
    /// Class for handling code input from files.
    /// </summary>
    internal class FileCodeInput : IRawCodeInput, IDisposable
    {
        private readonly ICoreComponentsMediator _mediator;
        protected StreamReader? Stream;

        public char? CurrentCharacter { get; private set; }

        public FileCodeInput(ICoreComponentsMediator mediator)
        {
            _mediator = mediator;
            _mediator.InputSourceRequested += OnInputSourceRequested;
        }

        public void MoveToNext()
        {
            if (Stream == null)
                throw new NotImplementedException("Implement error when stream is not initialized and character is advanced.");

            var character = Stream.Read();
            CurrentCharacter = character == -1 ? null : (char)character;
        }

        public void Dispose()
        {
            Stream?.Dispose();
        }

        private void OnInputSourceRequested(object? sender, InputSourceRequestedEventArgs e)
        {
            try
            {
                Stream = new StreamReader(e.SourcePath);
            }
            catch (IOException ex)
            {
                throw new NotImplementedException("Implement error when reading a file failed.");
            }
        }
    }
}
