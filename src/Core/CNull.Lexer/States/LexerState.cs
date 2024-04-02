using CNull.Lexer.Constants;
using CNull.Source;

namespace CNull.Lexer.States
{
    /// <summary>
    /// Base class for lexer states that interact with the source.
    /// </summary>
    /// <param name="source"></param>
    public abstract class LexerState(ICodeSource source) : ILexerState
    {
        protected ICodeSource Source = source;
        public abstract bool TryBuildToken(out Token token);

        protected bool TokenFailed(out Token token)
        {
            token = new Token(TokenType.End);
            Source.MoveToNext();
            return false;
        }
    }
}
