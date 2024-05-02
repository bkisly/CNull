using CNull.Parser.Productions;

namespace CNull.Parser
{
    /// <summary>
    /// Represents a syntactic analyser of C? programs.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Parses the program.
        /// </summary>
        /// <returns></returns>
        Program? Parse();
    }
}
