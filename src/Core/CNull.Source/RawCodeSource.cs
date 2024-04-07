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

        public char? CurrentCharacter { get; private set; }
        public bool IsCurrentCharacterNewLine => CurrentCharacter.HasValue && Environment.NewLine.Contains(CurrentCharacter.Value);

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
                _errorHandler.RaiseSourceError(new StreamNotInitializedError());

            var character = _inputRepository.Read();
            CurrentCharacter = character == -1 ? null : (char)character;
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

        private void OnSourceInitialized() => SourceInitialized?.Invoke(this, EventArgs.Empty);
    }
}
