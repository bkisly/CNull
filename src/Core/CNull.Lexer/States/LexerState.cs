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
        protected char? CurrentCharacter => Source.CurrentCharacter;

        public abstract bool TryBuildToken(out Token token);

        protected bool TokenFailed(out Token token)
        {
            token = new Token(TokenType.Unknown);
            SkipToken();
            return false;
        }

        protected virtual void SkipToken()
        {
            while(!TokenHelpers.IsTokenTerminator(Source.CurrentCharacter))
                Source.MoveToNext();

            // @TODO: handle situation when invalid token is too long
        }
    }
}
