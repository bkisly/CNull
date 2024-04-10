namespace CNull.Source.Repositories
{
    public class InputRepository : IInputRepository
    {
        private TextReader? _reader;

        public bool IsInitialized => _reader != null;

        public void SetupStream(TextReader reader) => _reader = reader;
        public void SetupFileStream(string path) => _reader = new StreamReader(path);

        public int Read() => _reader?.Read() ?? throw new InvalidOperationException("Tried to read from non-initialized stream.");
        public void Dispose() => _reader?.Dispose();
    }
}
