using CNull.Source.Repositories;

namespace CNull.Source.Raw
{
    /// <summary>
    /// Base class for handling raw code input logic.
    /// </summary>
    public class RawCodeInput(IInputRepository inputRepository) : IRawCodeInput, IDisposable
    {
        protected IInputRepository InputRepository = inputRepository;

        public char? CurrentCharacter { get; private set; }

        public void MoveToNext()
        {
            if (!InputRepository.IsInitialized)
                throw new NotImplementedException("Implement error when stream is not initialized and character is advanced.");

            var character = InputRepository.Read();
            CurrentCharacter = character == -1 ? null : (char)character;
        }

        public void Dispose() => InputRepository.Dispose();
    }
}
