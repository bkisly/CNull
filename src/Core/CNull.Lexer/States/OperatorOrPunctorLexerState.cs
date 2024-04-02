using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// Represents a state in which operator or punctor is built.
    /// </summary>
    public class OperatorOrPunctorLexerState(ICodeSource source) : LexerState(source)
    {
        public override bool TryBuildToken(out Token token)
        {
            throw new NotImplementedException();
        }
    }
}
