namespace CNull.Source.Repositories
{
    internal class InputRepository : IInputRepository
    {
        private StreamReader? _streamReader;

        public bool IsInitialized => _streamReader != null;
        public void SetupFileStream(string path) => _streamReader = new StreamReader(path);

        public int Read() => _streamReader?.Read() ?? throw new InvalidOperationException("Tried to read from non-initialized stream.");
        public void Dispose() => _streamReader?.Dispose();
    }
}
