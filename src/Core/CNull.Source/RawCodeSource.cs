using CNull.Common;
using CNull.Common.Events.Args;
using CNull.Common.Mediators;
using CNull.ErrorHandler;
using CNull.ErrorHandler.Errors.Source;
using CNull.Source.Repositories;

namespace CNull.Source
{
    /// <summary>
    /// Base class for handling raw code input logic.
    /// </summary>
    public class RawCodeSource : IRawCodeSource, IDisposable
    {
        private readonly IInputRepository _inputRepository;
        private readonly IErrorHandler _errorHandler;
        private readonly ICoreComponentsMediator _coreComponentsMediator;

        private char? _previousCharacter;

        public char? CurrentCharacter { get; private set; }
        public bool IsCurrentCharacterNewLine => IsNewLine(CurrentCharacter);

        private Position _position;
        public Position Position => _position;

        public event EventHandler? SourceInitialized;

        public RawCodeSource(IInputRepository inputRepository, IErrorHandler errorHandler,
            ICoreComponentsMediator coreComponentsMediator)
        {
            _inputRepository = inputRepository;
            _errorHandler = errorHandler;
            _coreComponentsMediator = coreComponentsMediator;

            _coreComponentsMediator.FileInputRequested += Mediator_FileInputRequested;
        }

        public void MoveToNext()
        {
            if (!_inputRepository.IsInitialized)
            {
                _errorHandler.RaiseSourceError(new StreamNotInitializedError());
                return;
            }

            _previousCharacter = CurrentCharacter;
            var character = _inputRepository.Read();
            CurrentCharacter = character == -1 ? null : (char)character;

            UpdatePosition();
        }

        public void Dispose() => _inputRepository.Dispose();

        private void Mediator_FileInputRequested(object? sender, FileInputRequestedEventArgs e)
        {
            try
            {
                _inputRepository.SetupFileStream(e.SourcePath);
                OnSourceInitialized();
            }
            catch (IOException)
            {
                _errorHandler.RaiseSourceError(new FileAccessError(e.SourcePath));
            }
        }

        private void UpdatePosition()
        {
            if (!CurrentCharacter.HasValue) 
                return;

            if (_previousCharacter == '\n')
            {
                _position.ColumnNumber = CurrentCharacter != '\r' ? 1 : 0;
                _position.LineNumber++;
            }
            else if (CurrentCharacter != '\r')
            {
                if (_position == default)
                    _position = new Position(1, 1);
                else _position.ColumnNumber++;
            }
        }

        private static bool IsNewLine(char? c) => c is '\n' or '\r';
        private void OnSourceInitialized() => SourceInitialized?.Invoke(this, EventArgs.Empty);
    }
}
