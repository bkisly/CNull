namespace CNull.Source
{
    /// <summary>
    /// Represents the source of a raw code.
    /// </summary>
    public interface ICodeSource
    {
        /// <summary>
        /// Returns the currently loaded character from the input source.
        /// </summary>
        char? CurrentCharacter { get; }

        /// <summary>
        /// Advances to the next character from the input source.
        /// </summary>
        void MoveToNext();
    }

    /// <summary>
    /// Represents a non filtered input code source.
    /// </summary>
    public interface IRawCodeSource : ICodeSource;
}
