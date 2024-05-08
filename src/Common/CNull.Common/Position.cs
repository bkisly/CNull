namespace CNull.Common
{
    /// <summary>
    /// Represents a position of an item in the analyzed code.
    /// </summary>
    /// <param name="LineNumber">Number of the line.</param>
    /// <param name="ColumnNumber">Number of the column.</param>
    public record struct Position(int LineNumber, int ColumnNumber)
    {
        /// <summary>
        /// Returns a position, which indicates at the first character.
        /// </summary>
        public static Position FirstCharacter => new(1, 1);

        public override string ToString() => $"({LineNumber}, {ColumnNumber})";
    }
}
