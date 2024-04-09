namespace CNull.Common
{
    /// <summary>
    /// Represents a position of an item in the analyzed code.
    /// </summary>
    /// <param name="LineNumber">Number of the line.</param>
    /// <param name="ColumnNumber">Number of the column.</param>
    public record struct Position(int LineNumber, int ColumnNumber);
}
