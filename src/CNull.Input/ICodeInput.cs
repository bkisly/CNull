namespace CNull.Input
{
    /// <summary>
    /// Represents the source of a raw code.
    /// </summary>
    internal interface ICodeInput
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
}
