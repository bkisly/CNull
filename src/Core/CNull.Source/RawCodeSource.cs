using CNull.Common;
using CNull.Common.Events;
using CNull.Common.Mediators;
using CNull.ErrorHandler;
using CNull.Source.Errors;

namespace CNull.Source
{
    /// <summary>
    /// Base class for handling raw code input logic.
    /// </summary>
    public class RawCodeSource : IRawCodeSource, IDisposable
    {
        private readonly IErrorHandler _errorHandler;
        private readonly ICoreComponentsMediator _coreComponentsMediator;

        private TextReader? _reader;
        private char? _previousCharacter;

        public char? CurrentCharacter { get; private set; }
        public char? NextCharacter
        {
            get
            {
                var character = _reader?.Peek() ?? -1;
                return character == -1 ? null : (char)character;
            }
        }

        public bool IsCurrentCharacterNewLine => IsNewLine(CurrentCharacter);

        private Position _position;
        public Position Position => _position;

        public event EventHandler? SourceInitialized;

        public RawCodeSource(IErrorHandler errorHandler, ICoreComponentsMediator coreComponentsMediator)
        {
            _errorHandler = errorHandler;
            _coreComponentsMediator = coreComponentsMediator;

            _coreComponentsMediator.InputRequested += Mediator_InputRequested;
        }

        public RawCodeSource(TextReader reader, IErrorHandler errorHandler, ICoreComponentsMediator mediator) 
            : this(errorHandler, mediator)
        {
            _reader = reader;
        }

        public void MoveToNext()
        {
            if (_reader == null)
            {
                _errorHandler.RaiseSourceError(new StreamNotInitializedError());
                return;
            }

            _previousCharacter = CurrentCharacter;
            var character = _reader.Read();
            CurrentCharacter = character == -1 ? null : (char)character;

            UpdatePosition();
        }

        public void Dispose() => _reader?.Dispose();

        private void Mediator_InputRequested(object? sender, InputRequestedEventArgs e)
        {
            try
            {
                _reader = e.Reader.Value;
                MoveToNext();
                OnSourceInitialized();
            }
            catch (IOException)
            {
                _errorHandler.RaiseSourceError(new InputAccessError(e.Path));
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
            else if (CurrentCharacter != '\r' || NextCharacter != '\n')
            {
                if (_position == default)
                    _position = Position.FirstCharacter;
                else _position.ColumnNumber++;
            }
        }

        private static bool IsNewLine(char? c) => c is '\n' or '\r';
        private void OnSourceInitialized() => SourceInitialized?.Invoke(this, EventArgs.Empty);
    }
}
