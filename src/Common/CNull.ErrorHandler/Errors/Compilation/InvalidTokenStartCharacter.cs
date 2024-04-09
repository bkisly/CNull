using CNull.Common;

namespace CNull.ErrorHandler.Errors.Compilation
{
    /// <summary>
    /// Indicates that currently analyzed token starts with invalid character.
    /// </summary>
    /// <param name="position"></param>
    public class InvalidTokenStartCharacter(Position position) : CompilationError(position)
    {
        public override string Message => "Analyzed lexical token starts with invalid character.";
    }
}
