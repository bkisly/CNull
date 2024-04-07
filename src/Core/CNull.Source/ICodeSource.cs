namespace CNull.Source
{
    /// <summary>
    /// Represents the source of a raw code.
    /// </summary>
    public interface ICodeSource
    {
        event EventHandler SourceInitialized;

        /// <summary>
        /// Returns the currently loaded character from the input source.
        /// </summary>
        char? CurrentCharacter { get; }

        /// <summary>
        /// Determines whether currently loaded character is recognized as a new line sequence part.
        /// </summary>
        bool IsCurrentCharacterNewLine { get; }

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
