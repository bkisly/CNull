using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// State in which integer or floating-point literal is built.
    /// </summary>
    public class NumericLexerState(ICodeSource source) : LexerState(source)
    {
        public override bool TryBuildToken(out Token token)
        {
            throw new NotImplementedException();
        }
    }
}
