using CNull.Common;

namespace CNull.Source
{
    /// <summary>
    /// Class responsible for unifying new line representation to '\n' form. Proxy layer between lexer and input.
    /// </summary>
    public class NewLineUnifierProxy : ICodeSource
    {
        private readonly IRawCodeSource _source;

        public NewLineUnifierProxy(IRawCodeSource source)
        {
            _source = source;
            _source.SourceInitialized += (_, _) => OnSourceInitialized();
        }

        public char? CurrentCharacter => _source.CurrentCharacter;
        public bool IsCurrentCharacterNewLine => _source.CurrentCharacter == '\n';
        public Position Position => _source.Position;

        public event EventHandler? SourceInitialized;

        public void MoveToNext()
        {
            _source.MoveToNext();

            if (CurrentCharacter == '\r' && _source.NextCharacter != '\n')
                _source.MoveToNext();
        }

        private void OnSourceInitialized() => SourceInitialized?.Invoke(this, EventArgs.Empty);
    }
}
