namespace CNull.Source.Repositories
{
    internal class InputRepository : IInputRepository
    {
        private StreamReader? _streamReader;

        public bool IsInitialized => _streamReader != null;
        public void Setup(Stream stream) => _streamReader = new StreamReader(stream);
        public int Read() => _streamReader?.Read() ?? throw new InvalidOperationException("Tried to read from non-initialized stream.");
        public void Dispose() => _streamReader?.Dispose();
    }
}
